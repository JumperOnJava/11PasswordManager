using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Password11.Datatypes;
using Password11.src.Ui;
using Password11.src.Util;
using Password11Lib.Util;

namespace Password11.ViewModel;

public class MainWindowModel : PropertyChangable
{

    public readonly Frame Navigator;
    private readonly ObservableCollection<UiTag> tags = new();

    public ObservableCollection<UiTag> Tags
    {
        get => tags;
    }

    private readonly ObservableCollection<UiAccount> accounts = new();

    public ObservableCollection<UiAccount> Accounts => accounts;

    private ObservableCollection<UiAccount> filteredAccounts = new();
    private ObservableCollection<UiAccount> FilteredAccounts => filteredAccounts;
    public ObservableCollection<UiAccount> DisplayAccounts => NoTagsSelected ? Accounts : FilteredAccounts;

    public SaveState saveState = SaveState.STATE_OK;

    public SaveState SaveState
    {
        get => saveState;
        set
        {
            saveState = value;
            if(saveState!= SaveState.STATE_FAIL)
            {
                LatestException = null;
            }
            onPropertyChanged();
            onPropertyChanged(nameof(LatestException));
        }
    }

    public bool isPaneOpen = true;

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
    
    private StorageManager manager ;
    public void SetNewManager(StorageManager manager)
    {
        this.manager = manager;
        tasks.Clear();
        AppSettings.GLOBAL.storageHistory.Add(this.manager);
        AppSettings.GLOBAL.Save();
        Save();
    }
    public readonly Action onClose;
    private readonly bool initialized;

    public MainWindowModel(StorageManager storageManager, StorageData data, Action closeCallback, Frame navigator)
    {
        
        onClose = closeCallback;
        manager = storageManager;
        Navigator = navigator;

        data.Tags.Select(tag => new UiTag(tag)).ToList().ForEach(Tags.Add);
        data.Accounts.Select(account => new UiAccount(account)).ToList().ForEach(Accounts.Add);

        Tags.CollectionChanged += (a1, a2) =>
        {
            UpdateVisualTags();
        };
        Accounts.CollectionChanged += (a1, a2) =>
        {
            UpdateVisualAccounts();
        };
        UpdateVisualTags();
        UpdateVisualAccounts();
        initialized = true;
    }

    private void UpdateVisualAccounts()
    {
        FilterAccounts();
        Save();
    }

    private void UpdateVisualTags()
    {
        Save();
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
        onPropertyChanged(nameof(NoTagsSelected));
        onPropertyChanged(nameof(Accounts));
        onPropertyChanged(nameof(FilteredAccounts));
        onPropertyChanged(nameof(DisplayAccounts));
        onPropertyChanged(nameof(Tags));

    }

    public List<Tag> RawTags { get; } = new();

    public void Save()  
    {
        if(!initialized)
            return;
        var newStorage = new StorageData();
        newStorage.Accounts = Accounts.Select(a => a.Target.CloneRef()).ToList();
        newStorage.Tags = Tags.Select(a => a.Target.CloneRef()).ToList();
        EnqueueSave(manager.SetData(newStorage));
        
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
                if (task.IsFaulted && ReferenceEquals(currentTaskId,latestTaskId))
                {
                    throw task.Exception;
                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                LatestException = e;
                this.SaveState = SaveState.STATE_FAIL;
                continue;
            }
            this.SaveState = SaveState.STATE_OK;
        }
    }


    internal void FilterAccounts()
    {
        FilteredAccounts.Clear();
        for (var index = 0; index < Accounts.Count; index++)
        {
            var indexRef = new { index };
            var account = Accounts[index];
            if (isFiltered(account.Target))
            {
                FilteredAccounts.Add(new UiAccount(account.Target));
            }
        }

    }

    private bool isFiltered(Account account)
    {
        foreach (var tag in Tags)
        {
            if (tag.Selected)
            {
                if (!tag.matches(account))
                {
                    return false;
                }
            }
        }

        return true;
    }

    internal void Exit()
    {
        Navigator.GoBack();
    }
    
}

public class SaveState
{
    public static readonly SaveState STATE_OK = new (new SymbolIcon(Symbol.Accept),new SolidColorBrush(Colors.Transparent));
    public static readonly SaveState STATE_FAIL = new (new SymbolIcon(Symbol.Cancel),new SolidColorBrush(Colors.OrangeRed));
    public static readonly SaveState STATE_UPLOAD = new (new SymbolIcon(Symbol.Upload),new SolidColorBrush(Colors.Gray));

    public IconElement Icon { get; }
    public Brush Brush { get; }
    private SaveState(IconElement icon, Brush brush)
    {
        Icon = icon;
        Brush = brush;
    }
}