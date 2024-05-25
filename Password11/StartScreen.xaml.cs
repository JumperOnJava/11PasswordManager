using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Password11.Datatypes;
using Password11.StorageDialogs.GlobalCreate;
using Password11.src.Util;
using Password11.Util;
using Password11.ViewModel;

namespace Password11
{
    public sealed partial class StartScreen
    {
        StartScreenModel model;

        private Frame navigator;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                this.navigator = e.Parameter as Frame;
                model = new StartScreenModel();
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException("Passed wrong class as page parameter", ex);
            }
        }
        public StartScreen()
        {
            this.InitializeComponent();
        }


        private async void OpenStorage(StorageManager storageManager)
        {
            if (!await storageManager.SetupManagerInGui(this))
            {
                return;
            }


            var tcs = new TaskCompletionSource<StorageData>();

            ContentDialog dialog=null;
            dialog = new DialogBuilder(this).Title($"Loading data from {storageManager.DisplayInfo.DisplayPath}").SecondaryButtonText("Cancel").AddSecondaryClickAction(
                () =>
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
                new DialogBuilder(this).Title("Failed to load data from storage").Content(dataTask.Exception?.Message).PrimaryButtonText("Ok").Build().ShowAsync();
                return;
            }
            if (tcs.Task.IsCanceled)
            {
                return;
            }
            var data = tcs.Task.Result;
            model.History.Add(new StartScreenModelStoragePath(storageManager));
            navigator.Navigate(typeof(AccountsListPage),
                new MainWindowModel(
                    storageManager,
                    data,
                    () => { },
                    navigator
                ),
                new DrillInNavigationTransitionInfo());
        }

        private void OpenRecentStorage(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            OpenStorage(((ButtonBase)sender).CommandParameter as StorageManager);
        }



        public class StartScreenModel
        {
            public static AppSettings AppSettings => AppSettings.settings;
            public ObservableCollection<StartScreenModelStoragePath> History { get; set; }
            public StartScreenModel()
            {
                AppSettings.Load();
                History = new ObservableCollection<StartScreenModelStoragePath>();
                foreach (var item in AppSettings.storageHistory)
                {
                    if (item.IsValid())
                        History.Add(new StartScreenModelStoragePath(item));
                }
                AppSettings.storageHistory.Sort((a, b) =>
                {
                    if (a.DisplayInfo.LastAccessTime == b.DisplayInfo.LastAccessTime) return 0;
                    return a.DisplayInfo.LastAccessTime > b.DisplayInfo.LastAccessTime ? 1 : -1;
                });
                NotifyCollectionChangedEventHandler action = (a, e) =>
                {
                    AppSettings.storageHistory.Clear();
                    foreach (var element in History.ToList())
                    {
                        AppSettings.storageHistory.Add(element.Manager);
                    }
                    AppSettings.Save();
                };
                History.CollectionChanged += action;
                action.Invoke(null, null);
            }
        }
        
        private void CreateDialog(object sender, RoutedEventArgs e)
        {
            var dialogCreator = new EmptyOperation<DialogManager>();
            var managerCreator = new EmptyOperation<StorageManager>();
            dialogCreator.OnResult += (success, result) =>
            {
                if (success)
                {
                    result.Start(this);
                }
            };
            managerCreator.OnResult += (success, result) =>
            {
                if (success)
                {
                    OpenStorage(result);
                }
            };
            
            var dialog = new CreateStorageDialog(dialogCreator, managerCreator);
            dialog.StartDialog(this);
        }
    }
    public class StartScreenModelStoragePath
    {
        public StorageManager Manager { get; }
        public string Name { get; }
        public string DisplayTime { get; set; }
        public string Path => Manager.DisplayInfo.DisplayPath;
        
        public StartScreenModelStoragePath(StorageManager manager)
        {
            this.Manager = manager;
            this.Name = manager.DisplayInfo.DisplayName;
            this.DisplayTime = manager.DisplayInfo.LastAccessTime.ToString("hh:mm dd/MM/yy");
        }
    }
}
