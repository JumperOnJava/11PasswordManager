using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using WinUi3Test;
using WinUi3Test.src;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Util;
using WinUi3Test.src.ViewModel;

public class MainWindowModel : PropertyChangable, WindowModel
{
    public readonly Frame navigator;
    private Storage storage;

    private ObservableCollection<UiTag> tags;
    public ObservableCollection<UiTag> Tags { get => tags; }

    private ObservableCollection<Account> accounts;
    public ObservableCollection<Account> Accounts { get => accounts; }
    public ObservableCollection<UiAccountModel> displayAccounts;
    public ObservableCollection<UiAccountModel> DisplayAccounts { get => displayAccounts; }
    public bool isPaneOpen = true;
    public bool IsPaneOpen
    {
        get => isPaneOpen; set
        {
            isPaneOpen = value;
            onPropertyChanged("IsPaneOpen");
        }
    }

    private UiAccountModel? currentEditAccount = null;
    private readonly Operation<Storage> operation;

    public MainWindowModel(Operation<Storage> storage,Frame navigator)
    {
        this.operation = storage;
        this.navigator = navigator;
        this.tags = new(storage.target.Tags.Map((e)=>new UiTag(e)));
        this.accounts = new(storage.target.Accounts);
        this.displayAccounts = new ObservableCollection<UiAccountModel>();
        Tags.CollectionChanged += (_,_) => onPropertyChanged("Tags");
        Accounts.CollectionChanged += (_, _) => FilterAccounts();
        FilterAccounts();
    }

    public void Save()
    {
        operation.target.Accounts.Clear();
        foreach (var account in Accounts)
        {
            operation.target.Accounts.Add(account);
            var tags = new List<Tag>(account.Tags);
            account.Tags.Clear();
            foreach(var tag in Tags)
                if (tag is UiTag)
                    account.Tags.Add(tag.Target);
                else
                    account.Tags.Add(tag);
        }
    
        operation.target.Tags.Clear();
        foreach (var tag in Tags)
        {
            operation.target.Tags.Add(tag.Target);
        }
        
        operation.Finish(true);
    }
    public void Cancel()
    {
        operation.Finish(false);
    }

    internal void FilterAccounts()
    {
        DisplayAccounts.Clear();
        foreach (var account in Accounts)
        {
            if (isFiltered(account))
            {
                DisplayAccounts.Add(new UiAccountModel(navigator, account));
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
}