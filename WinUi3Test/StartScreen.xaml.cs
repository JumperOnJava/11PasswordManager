using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Specialized;
using System.Threading.Tasks;
using WinUi3Test.Datatypes;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.Util;
using WinUi3Test.StorageDialogs.GlobalCreate;
using WinUi3Test.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test
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


        private void OpenStorage(StorageManager storageManager)
        {
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



        private void OpenDialog(object sender, RoutedEventArgs e)
        {
            BeginDialog((a, b) => new OpenStorageDialog(a, b));
        }

        private void CreateDialog(object sender, RoutedEventArgs e)
        {
            BeginDialog((a, b) => new CreateStorageDialog(a, b));
        }
        public void BeginDialog(Func<EmptyOperation<DialogPage>, EmptyOperation<StorageManager>, DialogPage> page)
        {
            var dialogCreator = new EmptyOperation<DialogPage>();
            var managerCreator = new EmptyOperation<StorageManager>();
            page.Invoke(dialogCreator, managerCreator).StartDialog(XamlRoot);
            dialogCreator.OnResult += (success, result) =>
            {
                result.StartDialog(XamlRoot);
            };
            managerCreator.OnResult += (success, result) =>
            {
                if (success)
                {
                    OpenStorage(result);
                }
            };
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
