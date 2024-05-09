using System;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WinUi3Test;
using WinUi3Test.Datatypes;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Util;
using WinUi3Test.src.ViewModel;
using WinUi3Test.ViewModel;

public class MainWindowModel : PropertyChangable
{
    public readonly Frame navigator;
    private ObservableCollection<UiTag> tags;

    public ObservableCollection<UiTag> Tags
    {
        get => tags;
    }

    private ObservableCollection<UiAccountModel> accounts;

    public ObservableCollection<UiAccountModel> Accounts
    {
        get => accounts;
    }

    public ObservableCollection<UiAccountModel> filteredAccounts;

    public ObservableCollection<UiAccountModel> FilteredAccounts
    {
        get => filteredAccounts;
    }

    public ObservableCollection<UiAccountModel> DispayAccounts => NoTagsSelected ? Accounts : FilteredAccounts; 

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
    
    private readonly Operation<Storage> operation;

    public MainWindowModel(Operation<Storage> storage, Frame navigator)
    {
        operation = storage;
        TagManager.Instance.tags = storage.target.Tags;
        this.navigator = navigator;

        accounts = new(storage.target.Accounts.Map(a => new UiAccountModel(this.navigator, AccountOperation.Start(a))));
        filteredAccounts = new ObservableCollection<UiAccountModel>();
        tags = new ObservableCollection<UiTag>();
        Tags.CollectionChanged += (_, _) =>
        {
            onPropertyChanged(nameof(NoTagsSelected));
            onPropertyChanged(nameof(DispayAccounts));
            onPropertyChanged(nameof(Tags));
            foreach (var uiTag in Tags)
            {
                uiTag.PropertyChanged += (_,_) => onPropertyChanged(nameof(NoTagsSelected));
            }
        };
        
        storage.target.TagsOrder.Map(r => new UiTag(r)).ForEach(Tags.Add);
        Accounts.CollectionChanged += (_, _) =>
        {
            FilterAccounts();
        };
        FilterAccounts();
    }

    public void Save()
    {
        var newStorage = new StaticStorage();
        newStorage.Accounts = new List<Account>(Accounts.Map(a=>a.Target.target));
        newStorage.TagsOrder = new List<TagRef>(Tags.Map(t => t.Target.Identifier));
        newStorage.TagsOrder.ForEach(element => newStorage.Tags[element.innerId] = element);
        newStorage.Tags = new Dictionary<long, Tag>(TagManager.Instance.tags);
        operation.target = newStorage;
        operation.Finish(true);
    }

    public void Cancel()
    {
        operation.Finish(false);
    }

    internal void FilterAccounts()
    {
        FilteredAccounts.Clear();
        for (var index = 0; index < Accounts.Count; index++)
        {
            var indexRef = new { index };
            var account = Accounts[index];
            var operation = AccountOperation.Start(account.Target);
            operation.OnFinished += acc =>
            {
                if (acc != null)
                {
                    //Console.WriteLine("ok");
                    Accounts[indexRef.index] = new UiAccountModel(navigator,AccountOperation.Start(acc));
                }
            };
            if (isFiltered(account.Target))
            {
                FilteredAccounts.Add(new UiAccountModel(navigator, operation));
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

    internal void ExitNoSave()
    {
        operation.Finish(false);
    }
}