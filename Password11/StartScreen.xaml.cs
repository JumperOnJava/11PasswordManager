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
using Password11.Datatypes;
using Password11.StorageDialogs.GlobalCreate;
using Password11.src.Util;

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
                    storageManager.Fail();
                    return;
                }

                model.History.Add(new StartScreenModelStoragePath(storageManager));
                navigator.Navigate(typeof(AccountsListPage),
                    new MainWindowModel(
                        storageManager,
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
            dialog.StartDialog(XamlRoot);
        }
    }
    public class StartScreenModelStoragePath
    {
        public StorageManager Manager { get; }
        public string Name { get; }
        public StartScreenModelStoragePath(StorageManager manager)
        {
            this.Manager = manager;
            this.Name = manager.DisplayInfo.DisplayPath;
        }
        public string Path => Manager.DisplayInfo.DisplayPath;
    }
}
