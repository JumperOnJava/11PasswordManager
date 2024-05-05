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
    public MainWindowModel(Storage storage,Frame navigator)
    {
        this.navigator = navigator;
        this.storage = storage;
        this.tags = new(storage.Tags.Map((e)=>new UiTag(e)));
        this.accounts = new(storage.Accounts);
        this.displayAccounts = new ObservableCollection<UiAccountModel>();
        Tags.CollectionChanged += (_,_) => onPropertyChanged("Tags");
        Accounts.CollectionChanged += (_, _) => FilterAccounts();
        FilterAccounts();
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