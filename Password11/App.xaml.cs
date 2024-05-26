using System;
using Windows.Graphics;
using Microsoft.UI.Xaml;
using Password11.Datatypes.Serializing;

namespace Password11;

public partial class App : Application
{
    public static MainWindow MainWindow = new();
    private Window m_window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Test.Start();
        MainWindow = new MainWindow();
        m_window = MainWindow;
        MainWindow.SizeChanged += (sender, e) =>
        {
            e.Handled = true;
            MainWindow.AppWindow.Resize(new SizeInt32(
                Math.Max(600, MainWindow.AppWindow.Size.Width),
                Math.Max(600, MainWindow.AppWindow.Size.Height)
            ));
        };

        MainWindow.Activate();
    }
}