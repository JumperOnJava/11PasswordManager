using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Password11.Datatypes;
using Password11.GUI;
using Password11.Util;
using Password11.ViewModel;
using CreateStorageDialog = Password11.GUI.StorageDialogs.GlobalCreate.CreateStorageDialog;

namespace Password11;

public sealed partial class StartScreen
{
    private StartScreenModel model;

    private Frame navigator;

    public StartScreen()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            navigator = e.Parameter as Frame;
            model = new StartScreenModel();
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("Passed wrong class as page parameter", ex);
        }
    }


    private async void OpenStorage(StorageManager storageManager)
    {
        var tcs = new TaskCompletionSource<StorageData>();

        var dialog = new DialogBuilder(this).Title($"Loading data from {storageManager.DisplayInfo.DisplayPath}")
            .SecondaryButtonText("Cancel").AddSecondaryClickAction(
                dialog =>
                {
                    tcs.SetCanceled();
                    dialog.Hide();
                }).Build();
        dialog.ShowAsync();
        var dataTask = storageManager.GetData();
        dataTask.ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully)
            {
                tcs.SetResult(t.Result);
                return;
            }

            if (t.IsFaulted)
            {
                tcs.SetResult(null);
                return;
            }

            throw new Exception("Should not be here");
        });

        await tcs.Task;
        dialog.Hide();

        if (tcs.Task.IsFaulted || tcs.Task.Result == null)
        {
            new DialogBuilder(this).Title("Failed to load data from storage").Content(dataTask.Exception?.Message)
                .PrimaryButtonText("Ok").Build().ShowAsync();
            return;
        }

        if (tcs.Task.IsCanceled) return;
        var data = tcs.Task.Result;
        model.History.Add(new StartScreenModelStoragePath(storageManager));
        navigator.Navigate(typeof(AccountsListPage),
            new AppListPageModel(
                storageManager,
                data,
                () => { },
                navigator
            ),
            new DrillInNavigationTransitionInfo());
    }

    private async void OpenStorageWithSetup(object sender, RoutedEventArgs e)
    {
        var storageManager = ((ButtonBase)sender).CommandParameter as StorageManager;
        if (!await storageManager.SetupManagerInGui(this))
        {
            storageManager.ResetOnFail();
            return;
        }

        OpenStorage(storageManager);
    }

    private void CreateDialog(object sender, RoutedEventArgs e)
    {
        CreateStorageDialog.CreateManager(this, OpenStorage);
    }
}

public class StartScreenModel
{
    public StartScreenModel()
    {
        AppSettings.GLOBAL.Load();
        var history = new ObservableCollection<StartScreenModelStoragePath>();
        History = history;
        foreach (var item in AppSettings.storageHistory)
            if (item.IsValid())
                History.Add(new StartScreenModelStoragePath(item));
        AppSettings.storageHistory.Sort((a, b) =>
        {
            if (a.DisplayInfo.LastAccessTime == b.DisplayInfo.LastAccessTime) return 0;
            return a.DisplayInfo.LastAccessTime > b.DisplayInfo.LastAccessTime ? 1 : -1;
        });
        history.CollectionChanged += UpdateHistory;
        UpdateHistory(null, null);
    }

    public static AppSettings AppSettings => AppSettings.GLOBAL;
    public Visibility HistoryVisibility => History.Any() ? Visibility.Visible : Visibility.Collapsed;
    public Visibility CreateVisibility => History.Any() ? Visibility.Collapsed : Visibility.Visible;
    public IList<StartScreenModelStoragePath> History { get; set; }

    private void UpdateHistory(object a, NotifyCollectionChangedEventArgs e)
    {
        AppSettings.storageHistory.Clear();
        foreach (var element in History.ToList()) AppSettings.storageHistory.Add(element.Manager);
        AppSettings.GLOBAL.Save();
    }
}

public class StartScreenModelStoragePath
{
    public StartScreenModelStoragePath(StorageManager manager)
    {
        Manager = manager;
        Name = manager.DisplayInfo.DisplayName;
        DisplayTime = manager.DisplayInfo.LastAccessTime.ToString("hh:mm dd/MM/yy");
    }

    public StorageManager Manager { get; }
    public string Name { get; }
    public string DisplayTime { get; set; }
    public string Path => Manager.DisplayInfo.DisplayPath;
}