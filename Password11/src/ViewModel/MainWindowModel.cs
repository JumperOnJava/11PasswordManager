using System;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Security.Isolation;
using Password11;
using Password11.Datatypes;
using Password11.src.Util;
using Password11.src.ViewModel;
using Password11.ViewModel;

public class MainWindowModel : PropertyChangable
{
    public readonly Frame navigator;
    private readonly ObservableCollection<UiTag> tags = new();

    public ObservableCollection<UiTag> Tags
    {
        get => tags;
    }

    private readonly ObservableCollection<UiAccount> accounts = new();

    public ObservableCollection<UiAccount> Accounts => accounts;

    public ObservableCollection<UiAccount> displayAccounts = new();
    public ObservableCollection<UiAccount> DispayAccounts => displayAccounts; 

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
    
    private readonly StorageManager storageManager;
    public readonly Action onClose;
    
    public MainWindowModel(StorageManager storageManager, Action closeCallback, Frame navigator)
    {
        this.onClose = closeCallback;
        this.storageManager = storageManager;
        this.navigator = navigator;
        
        this.storageManager.Data.TagsList().Select(tag => new UiTag(tag)).ToList().ForEach(Tags.Add);
        this.storageManager.Data.AccountsList().Select(account => new UiAccount(this.navigator, account, RawTags)).ToList().ForEach(Accounts.Add);
        
        Tags.CollectionChanged += (_, _) =>
        {
            onPropertyChanged(nameof(NoTagsSelected));
            onPropertyChanged(nameof(DispayAccounts));
            onPropertyChanged(nameof(Tags));
            RawTags.Clear();
            foreach (var uiTag in Tags)
            {
                uiTag.PropertyChanged += (_,_) => onPropertyChanged(nameof(NoTagsSelected));
                RawTags.Add(uiTag.Target);
            }
            
            Save();
        };
        
        Accounts.CollectionChanged += (_, _) =>
        {
            FilterAccounts();
            Save();
        };
        FilterAccounts();
    }

    public List<Tag> RawTags { get; set; } = new();

    public void Save()  
    {
        var newStorage = new StorageData();
        newStorage.Accounts = new Dictionary<long, Account>();
        foreach (var uiAccount in Accounts)
        {
            newStorage.Accounts[uiAccount.Identifier.id] = uiAccount.Target;
        }
        newStorage.Tags = new Dictionary<long, Tag>();
        foreach (var uiTag in Tags)
        {
            newStorage.Tags[uiTag.Identifier.id] = uiTag.Target;
        }

        newStorage.AccountsOrder = Accounts.Select(a => a.Target.Identifier.id).ToList();
        newStorage.TagsOrder = Tags.Select(a => a.Target.Identifier.id).ToList();
        storageManager.Data = newStorage;
    }

    internal void FilterAccounts()
    {
        DispayAccounts.Clear();
        for (var index = 0; index < Accounts.Count; index++)
        {
            var indexRef = new { index };
            var account = Accounts[index];
            if (isFiltered(account.Target))
            {
                DispayAccounts.Add(new UiAccount(navigator, account.Target,this.RawTags));
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
        navigator.GoBack();
    }
}