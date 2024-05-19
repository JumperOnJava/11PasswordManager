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
        private async void CreateNew(object sender, RoutedEventArgs e)
        {
            if (_opening) return;
            _opening = true;
            try
            {
                var saveFIleDialog = new FileSavePicker();
                var window = App.MainWindow;

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                WinRT.Interop.InitializeWithWindow.Initialize(saveFIleDialog, hWnd);

                saveFIleDialog.FileTypeChoices.Add("Password storage", new List<string> { ".pwdb" });

                var file = await saveFIleDialog.PickSaveFileAsync();
                if (file != null)
                {
                    var pw = await AskPassword(true);
                    Extensions.ShowExceptionOnFail( (() =>
                    {
                        var saveLoader = new FileSaveLoader(file.Path).AesEncryptedStorage(pw);
                        OpenStorage(new StorageManager(saveLoader,new StorageData()));
                    }), XamlRoot);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            _opening = false;
        }

        private static bool _opening;
        private async void OpenStorageDialog(object sender, RoutedEventArgs e)
        {
            if (_opening) return;
            _opening = true;
            try
            {
                var openFileDialog = new FileOpenPicker();

                var window = App.MainWindow;

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                WinRT.Interop.InitializeWithWindow.Initialize(openFileDialog, hWnd);

                openFileDialog.FileTypeFilter.Add(".pwdb");
                openFileDialog.FileTypeFilter.Add("*");

                var file = await openFileDialog.PickSingleFileAsync();
                if (file != null)
                {
                    OpenStorage(file.Path);
                }
                else
                {
                    Console.WriteLine("Operation cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _opening = false;
        }

        public async void OpenStorage(string path)
        {
            
            var pw = await AskPassword(false);
            if (pw == null) return;
            Extensions.ShowExceptionOnFail( (() =>
            {
                OpenStorage(new StorageManager(new FileSaveLoader(path).AesEncryptedStorage(pw)));
            }), XamlRoot);
        }

        private void OpenStorage(StorageManager saveableStorage)
        {
                navigator.Navigate(typeof(AccountsListPage),
                    new MainWindowModel(
                        saveableStorage,
                        () => model.History.Add(new StartScreenModelStoragePath(saveableStorage.DataLoader)),
                        navigator
                        ),
                    new DrillInNavigationTransitionInfo());
        }

        private void OpenRecentStorage(object sender, RoutedEventArgs e)
        {
            OpenStorage(((ButtonBase)sender).CommandParameter as string);
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
                    if(item.IsValid())
                        History.Add(new StartScreenModelStoragePath(item));
                }
                AppSettings.storageHistory.Sort((a, b) =>
                {
                    if (a.LastAccessTime == b.LastAccessTime) return 0;
                    return a.LastAccessTime > b.LastAccessTime ? 1 : -1;
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
                action.Invoke(null,null);
            }
        }

        public async Task<string> AskPassword(bool hasSecondField)
        {
            var dialog = new ContentDialog();
            dialog.Title = "Enter password:";
            dialog.PrimaryButtonText = "Confirm";
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.SecondaryButtonText = "Cancel";
            dialog.XamlRoot = this.XamlRoot;
            dialog.Content = new PasswordInputDialog(dialog,hasSecondField);
            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                return ((PasswordInputDialog)dialog.Content).model.Password;
            }
            return null;
        }

        
    }
    public class StartScreenModelStoragePath
    {
        public SaveLoader Manager { get; set; } 
        public string Name { get; set; }
        public StartScreenModelStoragePath(SaveLoader manager)
        {
            this.Manager = manager;
            this.Name = System.IO.Path.GetFileNameWithoutExtension(manager.DisplayPath);
        }

        public string Path => Manager.DisplayPath;
    }
}
