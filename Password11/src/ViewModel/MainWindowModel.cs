using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.src.Util;

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
    private ObservableCollection<UiAccount> FilteredAccounts => filteredAccounts ;
    public ObservableCollection<UiAccount> DispayAccounts => NoTagsSelected ? Accounts : FilteredAccounts; 

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
    
    private readonly StorageManager manager;
    public readonly Action onClose;
    private readonly bool initialized;

    public MainWindowModel(StorageManager storageManager, StorageData data, Action closeCallback, Frame navigator)
    {
        onClose = closeCallback;
        manager = storageManager;
        Navigator = navigator;

        data.Tags.Select(tag => new UiTag(tag)).ToList().ForEach(Tags.Add);
        data.Accounts.Select(account => new UiAccount(account)).ToList().ForEach(Accounts.Add);

        Tags.CollectionChanged += (_, _) => UpdateVisualTags();
        Accounts.CollectionChanged += (_, _) => UpdateVisualAccounts();
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
        onPropertyChanged(nameof(NoTagsSelected));
        onPropertyChanged(nameof(DispayAccounts));
        onPropertyChanged(nameof(Tags));
        RawTags.Clear();
        foreach (var uiTag in Tags)
        {
            uiTag.PropertyChanged += (_, _) => onPropertyChanged(nameof(NoTagsSelected));
            RawTags.Add(uiTag.Target);
        }
        onPropertyChanged("DisplayAccounts");
        Save();
    }

    public List<Tag> RawTags { get; } = new();

    public void Save()  
    {
        if(!initialized)
            return;
        var newStorage = new StorageData();
        newStorage.Accounts = Accounts.Select(a => a.Target.CloneRef()).ToList();
        newStorage.Tags = Tags.Select(a => a.Target.CloneRef()).ToList();
        manager.SetData(newStorage);
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