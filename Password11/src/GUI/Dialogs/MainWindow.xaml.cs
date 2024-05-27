using System;
using Windows.Foundation;
using Windows.Graphics;
using Microsoft.UI.Input;
using Password11.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Password11.GUI.Dialogs;

/// <summary>
///     An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        RootGrid.SizeChanged += (_, _) => SetRegionsForCustomTitleBar();
        RootGrid.Loaded += (_, _) => SetRegionsForCustomTitleBar();
        ExtendsContentIntoTitleBar = true;

        ContentFrame.Navigate(typeof(StartScreen), ContentFrame);
    }

    public AppListPageModel Model { get; set; }

    private void SetRegionsForCustomTitleBar()
    {
        var scaleAdjustment = RootGrid.XamlRoot.RasterizationScale;
        var bounds = new Rect(0, 0, 100, 40);
        var searchBoxRect = new RectInt32(
            (int)Math.Round(bounds.X * scaleAdjustment),
            (int)Math.Round(bounds.Y * scaleAdjustment),
            (int)Math.Round(bounds.Width * scaleAdjustment),
            (int)Math.Round(bounds.Height * scaleAdjustment)
        );
        var rectArray = new[] { searchBoxRect };
        var nonClientInputSrc =
            InputNonClientPointerSource.GetForWindowId(AppWindow.Id);
        nonClientInputSrc.SetRegionRects(NonClientRegionKind.Passthrough, rectArray);
    }
}