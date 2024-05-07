using System;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    private ObservableCollection<Account> accounts;

    public ObservableCollection<Account> Accounts
    {
        get => accounts;
    }

    public ObservableCollection<UiAccountModel> displayAccounts;

    public ObservableCollection<UiAccountModel> DisplayAccounts
    {
        get => displayAccounts;
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

    private UiAccountModel? currentEditAccount = null;
    private readonly Operation<Storage> operation;

    public MainWindowModel(Operation<Storage> storage, Frame navigator)
    {
        this.operation = storage;
        TagManager.Instance.tags = storage.target.Tags;
        this.navigator = navigator;
        this.tags = new ObservableCollection<UiTag>(
            storage.target.TagsOrder
                .Map(r => new UiTag(r))
        );


        this.accounts = new(storage.target.Accounts);
        this.displayAccounts = new ObservableCollection<UiAccountModel>();
        Tags.CollectionChanged += (_, _) => onPropertyChanged("Tags");
        Accounts.CollectionChanged += (_, _) => FilterAccounts();
        FilterAccounts();
    }

    public void Save()
    {
        var newStorage = new StaticStorage();
        newStorage.Accounts = new List<Account>(Accounts);
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
        DisplayAccounts.Clear();
        for (var index = 0; index < Accounts.Count; index++)
        {
            var indexRef = new { index };
            var account = Accounts[index];
            if (isFiltered(account))
            {
                var operation = AccountOperation.Start(account);
                operation.OnFinished += acc =>
                {
                    if (acc != null)
                    {
                        //Console.WriteLine("ok");
                        Accounts[indexRef.index] = acc;
                    }
                };
                DisplayAccounts.Add(new UiAccountModel(navigator, operation));

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