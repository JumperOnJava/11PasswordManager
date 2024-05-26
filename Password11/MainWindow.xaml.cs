using System;
using Windows.Foundation;
using Windows.Graphics;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Password11.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Password11;

/// <summary>
///     An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public AppWindow m_AppWindow;

    public MainWindow()
    {
        //Map((i) => new AccountEntry(i, (it) => model.CurrentEditAccount = it.Clone())));

        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        m_AppWindow = AppWindow;
        RootGrid.SizeChanged += (f, f2) => SetRegionsForCustomTitleBar();
        RootGrid.Loaded += (f3, f4) => SetRegionsForCustomTitleBar();
        ExtendsContentIntoTitleBar = true;

        ContentFrame.Navigate(typeof(StartScreen), ContentFrame);
    }

    public MainWindowModel model { get; set; }

    private void SetRegionsForCustomTitleBar()
    {
        var scaleAdjustment = RootGrid.XamlRoot.RasterizationScale;
        var bounds = new Rect(0, 0, 100, 40);
        var SearchBoxRect = new RectInt32(
            (int)Math.Round(bounds.X * scaleAdjustment),
            (int)Math.Round(bounds.Y * scaleAdjustment),
            (int)Math.Round(bounds.Width * scaleAdjustment),
            (int)Math.Round(bounds.Height * scaleAdjustment)
        );
        var rectArray = new[] { SearchBoxRect };
        var nonClientInputSrc =
            InputNonClientPointerSource.GetForWindowId(AppWindow.Id);
        nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
    }
}