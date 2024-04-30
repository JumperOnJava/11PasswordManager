using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_AppWindow;
        public static MainWindowModel StaticModel;
        public MainWindowModel model { get => MainWindow.StaticModel; set => MainWindow.StaticModel = value; }
        public MainWindow()
        {
            //Map((i) => new AccountEntry(i, (it) => model.CurrentEditAccount = it.Clone())));
            this.InitializeComponent();
            model = new MainWindowModel(StaticStorage.instance,this.ContentFrame);

            ContentFrame.Navigate(typeof(AccountsListPage), model);
            
            ExtendsContentIntoTitleBar = true;
            m_AppWindow = this.AppWindow;
            AppTitleBar.SizeChanged += (f, f2) => SetRegionsForCustomTitleBar();
            AppTitleBar.Loaded += (f3, f4) => SetRegionsForCustomTitleBar();
            ExtendsContentIntoTitleBar = true;
        }
        private void SetRegionsForCustomTitleBar()
        {
            double scaleAdjustment = AppTitleBar.XamlRoot.RasterizationScale;
            GeneralTransform transform = menuBar_main.TransformToVisual(null);
            Rect bounds = transform.TransformBounds(new Rect(0, 0, 100, 32));
            Windows.Graphics.RectInt32 SearchBoxRect = new Windows.Graphics.RectInt32(
                _X: (int)Math.Round(bounds.X * scaleAdjustment),
                _Y: (int)Math.Round(bounds.Y * scaleAdjustment),
                _Width: (int)Math.Round(bounds.Width * scaleAdjustment),
                _Height: (int)Math.Round(bounds.Height * scaleAdjustment)
            );
            var rectArray = new Windows.Graphics.RectInt32[] { SearchBoxRect };
            InputNonClientPointerSource nonClientInputSrc =
                InputNonClientPointerSource.GetForWindowId(this.AppWindow.Id);
            nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
        }
    }

}
