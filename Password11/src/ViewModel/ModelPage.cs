using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Password11.ViewModel;

namespace Password11.src.ViewModel;

internal class ModelPage : Page
{
    public MainWindowModel model;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            model = e.Parameter as MainWindowModel;
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("Passed wrong class as page parameter", ex);
        }
    }
}