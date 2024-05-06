using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using WinUi3Test.src.ViewModel;
using WinUi3Test.Storage;

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
                if (File.Exists("settings"))
                    model.appSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("settings"));
                else model.appSettings = new AppSettings();
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
        public class StartScreenModel
        {
            public AppSettings appSettings;
        }

        private void CreateNew(object sender, RoutedEventArgs e)
        {

        }

        private static bool opening = false;
        private void OpenStorage(object sender, RoutedEventArgs e)
        {
            try
            {
                if (opening) return;
                opening = true;
                var openFileDialog = new FileOpenPicker();

                var window = App.MainWindow;

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                WinRT.Interop.InitializeWithWindow.Initialize(openFileDialog, hWnd);

                openFileDialog.FileTypeFilter.Add("*");
                openFileDialog.PickSingleFileAsync();
                var file = openFileDialog.PickSingleFileAsync().GetResults();
                Console.WriteLine(file.Name);
                if (file != null)
                {
                    Console.WriteLine("Picked file: " + file.Name);
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
    }
}
