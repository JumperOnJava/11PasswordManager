using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinUi3Test;
using WinUi3Test.src;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Util;
using WinUi3Test.src.ViewModel;

public class MainWindowModel : INotifyPropertyChanged, WindowModel
{
    private readonly Frame navigator;
    private Storage storage;

    private ObservableCollection<Tag> tags;
    public ObservableCollection<Tag> Tags { get => tags; }

    private ObservableCollection<UiAccount> accounts;
    public ObservableCollection<UiAccount> Accounts { get => accounts; }

    private UiAccount? currentEditAccount = null;
    public UiAccount? CurrentEditAccount
    {
        get => currentEditAccount;
        set
        {
            currentEditAccount = value;
            if(currentEditAccount != null && navigator!=null)
            {
                 navigator.Navigate(typeof(PasswordEdit));
            }
            else
            {
                navigator.GoBack();
            }
            onPropertyChanged("CurrentEditAccount");
            onPropertyChanged("CurrentAccountBeingEdited");
        }
    }
    public bool CurrentAccountBeingEdited {
        get
        {
            return currentEditAccount != null;
        }
    }
    public MainWindowModel(Storage storage,Frame navigator)
    {
        this.navigator = navigator;
        this.storage = storage;
        this.tags = new(storage.Tags);
        this.accounts = new(storage.Accounts.Map(e => new UiAccountImpl(this,e.TargetApp, e.Username, e.Password)));
        this.CurrentEditAccount = accounts[0];
        Tags.CollectionChanged += (_,_) => onPropertyChanged("Tags");
        Accounts.CollectionChanged += (_,_) => onPropertyChanged("Accounts");
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private void onPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
    }
}