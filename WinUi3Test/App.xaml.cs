using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.UI.ViewManagement;
using WinUi3Test.Datatypes.Serializing;

namespace WinUi3Test
{
    public partial class App : Application
    {
        public static MainWindow MainWindow = new();
        public App()
        {
            this.InitializeComponent();
        }
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Test.Start();
            MainWindow = new MainWindow();
            m_window = MainWindow;
            MainWindow.SizeChanged += (sender,e) =>
            {
                e.Handled = true;
                MainWindow.AppWindow.Resize(new SizeInt32(
                    Math.Max(600,MainWindow.AppWindow.Size.Width),
                    Math.Max(600,MainWindow.AppWindow.Size.Height)
                ));
            };
            
            MainWindow.Activate();
        }
        private Window m_window;

    }
}
