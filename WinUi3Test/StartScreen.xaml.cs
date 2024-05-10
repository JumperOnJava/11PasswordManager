using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml.Media.Animation;
using WinUi3Test.src.Storage;
using WinUi3Test.src.ViewModel;
using WinUi3Test.Storage;
using System.Collections.Specialized;
using System.Threading.Tasks;
using WinUi3Test.Datatypes;
using WinUi3Test.src.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test
{
    public sealed partial class StartScreen : Page
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
            if (opening) return;
            opening = true;
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
                    var pw = await askPassword(true);
                    OpenStorage(Encryption.Encrypt(JsonSerializer.Serialize(new FileStorage(), Test.JsonOption),pw), file.Path,pw);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            opening = false;
        }

        private static bool opening;
        private async void OpenStorageDialog(object sender, RoutedEventArgs e)
        {
            if (opening) return;
            opening = true;
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
            opening = false;
        }

        public async void OpenStorage(string path)
        {
            var pw = await askPassword(false);
            OpenStorage(File.ReadAllBytes(path), path, pw);
        }

        private void OpenStorage(byte[] data, string path, string password)
        {
            if(password == null)
            {
                return;
            }
            try
            {
                FileStorage staticStorage = JsonSerializer.Deserialize<FileStorage>(Encryption.Decrypt(data,password), Test.JsonOption);
                var storageOperation = new Operation<Datatypes.Storage>(staticStorage);
                navigator.Navigate(typeof(AccountsListPage), new MainWindowModel(storageOperation, navigator),
                    new DrillInNavigationTransitionInfo());
                storageOperation.onFinished += s =>
                {
                    if (s != null)
                    {
                        model.history.Add(new StartScreenModelStoragePath(path));
                        File.WriteAllBytes(path,Encryption.Encrypt(JsonSerializer.Serialize(s, Test.JsonOption), password));
                    }
                    navigator.GoBack();
                };
            }
            catch(Exception e)
            {
                var dialog = new ContentDialog();
                dialog.PrimaryButtonText = "Ok";
                dialog.Title = "Error reading file";
                dialog.Content = e.StackTrace;
                dialog.XamlRoot = this.XamlRoot;
                dialog.ShowAsync();
                //throw new Exception("",e);
            }
        }

        private void OpenRecentStorage(object sender, RoutedEventArgs e)
        {
            OpenStorage(((ButtonBase)sender).CommandParameter as string);
        }
        public class StartScreenModel
        {
            public AppSettings appSettings => AppSettings.settings;
            public ObservableCollection<StartScreenModelStoragePath> history { get; set; }
            public StartScreenModel()
            {
                AppSettings.Load();
                history = new ObservableCollection<StartScreenModelStoragePath>();
                foreach (var item in appSettings.storageHistory)
                {
                    if(File.Exists(item))
                        history.Add(new StartScreenModelStoragePath(item));
                }
                appSettings.storageHistory.Sort((a, b) =>
                {
                    if (File.GetLastAccessTime(a) == File.GetLastAccessTime(b)) return 0;
                    return File.GetLastAccessTime(a) > File.GetLastAccessTime(b) ? 1 : -1;
                });
                NotifyCollectionChangedEventHandler action = (a, e) =>
                {
                    appSettings.storageHistory.Clear();
                    history.ToList().ForEach(element => appSettings.storageHistory.Add(element.Path));
                    AppSettings.Save();
                };
                history.CollectionChanged += action;
                action.Invoke(null,null);
            }
        }

        public async Task<string> askPassword(bool isDouble)
        {
            var dialog = new ContentDialog();
            dialog.Title = "Enter password:";
            dialog.PrimaryButtonText = "Confirm";
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.SecondaryButtonText = "Cancel";
            dialog.XamlRoot = this.XamlRoot;
            dialog.Content = new PasswordInputDialog(dialog,isDouble);
            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                return (dialog.Content as PasswordInputDialog).model.Password;
            }
        
            return null;
        }
    }
    public class StartScreenModelStoragePath
    {
        public string Path { get; set; } 
        public string Name { get; set; }
        public StartScreenModelStoragePath(string path)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }
}
