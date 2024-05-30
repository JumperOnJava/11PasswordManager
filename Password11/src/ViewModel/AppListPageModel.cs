using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Password11.Datatypes;
using Password11.src.Util;
using Password11.StorageManagers;
using Password11Lib.Util;

namespace Password11.ViewModel;

public class AccountListPageModel : PropertyChangable
{
    private readonly bool initialized;

    public readonly Frame Navigator;
    public readonly Action onClose;


    public bool isPaneOpen = true;

    public StorageManager Manager { get; private set; }

    public SaveState saveState = SaveState.STATE_OK;

    public AccountListPageModel(StorageManager storageManager, StorageData data, Action closeCallback, Frame navigator)
    {
        onClose = closeCallback;
        Manager = storageManager;
        Navigator = navigator;

        data.Tags.Select(tag => new UiTag(tag)).ToList().ForEach(Tags.Add);
        data.Accounts.Select(account => new UiAccount(account)).ToList().ForEach(Accounts.Add);

        Tags.CollectionChanged += (a1, a2) => { UpdateVisualTags(); };
        Accounts.CollectionChanged += (a1, a2) => { UpdateVisualAccounts(); };
        UpdateVisualTags();
        UpdateVisualAccounts();
        initialized = true;
    }

    public ObservableCollection<UiTag> Tags { get; } = new();

    public ObservableCollection<UiAccount> Accounts { get; } = new();

    public ObservableCollection<UiAccount> FilteredAccounts { get; } = new();

    public ObservableCollection<UiAccount> DisplayAccounts => NoTagsSelected ? Accounts : FilteredAccounts;

    public SaveState SaveState
    {
        get => saveState;
        set
        {
            saveState = value;
            if (saveState != SaveState.STATE_FAIL) LatestException = null;
            onPropertyChanged();
            onPropertyChanged(nameof(LatestException));
        }
    }

    public bool IsPaneOpen
    {
        get => isPaneOpen;
        set
        {
            isPaneOpen = value;
            onPropertyChanged();
        }
    }

    public bool NoTagsSelected => !Tags.Any(t => t.Selected);

    public List<Tag> RawTags { get; } = new();

    public void SetNewManager(StorageManager manager)
    {
        this.Manager = manager;
        onPropertyChanged(nameof(this.Manager));
        tasks.Clear();
        AppSettings.GLOBAL.storageHistory.Add(this.Manager);
        AppSettings.GLOBAL.Save();
        Save();
    }

    private void UpdateVisualAccounts()
    {
        FilterAccounts();
        Save();
    }

    private void UpdateVisualTags()
    {
        RawTags.Clear();
        foreach (var uiTag in Tags)
        {
            uiTag.PropertyChanged += (_, _) =>
            {
                onPropertyChanged(nameof(NoTagsSelected));
                onPropertyChanged(nameof(DisplayAccounts));
            };
            RawTags.Add(uiTag.Target);
        }
        FilterAccounts();
        Save();
        onPropertyChanged(nameof(NoTagsSelected));
        onPropertyChanged(nameof(Accounts));
        onPropertyChanged(nameof(FilteredAccounts));
        onPropertyChanged(nameof(DisplayAccounts));
        onPropertyChanged(nameof(Tags));
    }

    public void Save()
    {
        if (!initialized)
            return;
        var newStorage = new StorageData();
        newStorage.Accounts = Accounts.Select(a => a.Target.CloneRef()).ToList();
        newStorage.Tags = Tags.Select(a => a.Target.CloneRef()).ToList();
        EnqueueSave(Manager.SetData(newStorage));
    }

    private Queue<Task> tasks = new();
    public Exception LatestException { get; set; }

    private UniqueId<object> latestTaskId;
    private async void EnqueueSave(Task func)
    {
        if (tasks.Any())
        {
            tasks.Enqueue(func);
            return;
        }

        tasks.Enqueue(func);
        while (tasks.Any())
        {
            SaveState = SaveState.STATE_UPLOAD;
            var task = tasks.Dequeue();
            var currentTaskId =  UniqueId<object>.CreateRandom<object>();
            latestTaskId = currentTaskId ;
            try
            {
                await task;
                if (task.IsFaulted && latestTaskId.id == currentTaskId.id)
                {
                    throw task.Exception;
                }
            }
            catch (Exception e)
            {
                LatestException = e;
                SaveState = SaveState.STATE_FAIL;
                continue;
            }

            SaveState = SaveState.STATE_OK;
        }
    }


    internal void FilterAccounts()
    {
        FilteredAccounts.Clear();
        for (var index = 0; index < Accounts.Count; index++)
        {
            var indexRef = new { index };
            var account = Accounts[index];
            if (isFiltered(account.Target)) FilteredAccounts.Add(new UiAccount(account.Target));
        }
    }

    private bool isFiltered(Account account)
    {
        foreach (var tag in Tags)
            if (tag.Selected)
                if (!tag.matches(account))
                    return false;

        return true;
    }

    internal void Exit()
    {
        Navigator.GoBack();
    }
}

public class SaveState
{
    public static readonly SaveState STATE_OK = new(new SymbolIcon(Symbol.Accept),
        new SolidColorBrush(Colors.Transparent));

    public static readonly SaveState STATE_FAIL = new(new SymbolIcon(Symbol.Cancel),
        new SolidColorBrush(Colors.OrangeRed));

    public static readonly SaveState
        STATE_UPLOAD = new(new SymbolIcon(Symbol.Upload), new SolidColorBrush(Colors.Gray));

    private SaveState(IconElement icon, Brush brush)
    {
        Icon = icon;
        Brush = brush;
    }

    public IconElement Icon { get; }
    public Brush Brush { get; }
}