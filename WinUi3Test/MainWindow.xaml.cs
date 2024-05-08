using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using Windows.Foundation;
using Windows.Security.Isolation;
using WinUi3Test.src.Storage;
using WinUi3Test.src.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public AppWindow m_AppWindow;
        public MainWindowModel model { get; set; }

        public MainWindow()
        {
            //Map((i) => new AccountEntry(i, (it) => model.CurrentEditAccount = it.Clone())));
           
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            m_AppWindow = this.AppWindow;
            RootGrid.SizeChanged += (f, f2) => SetRegionsForCustomTitleBar();
            RootGrid.Loaded += (f3, f4) => SetRegionsForCustomTitleBar();
            ExtendsContentIntoTitleBar = true;
    
            ContentFrame.Navigate(typeof(StartScreen), ContentFrame);
        }
        private void SetRegionsForCustomTitleBar()
        {
            double scaleAdjustment = this.RootGrid.XamlRoot.RasterizationScale;
            Rect bounds = new Rect(0, 0, 100, 40);
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
