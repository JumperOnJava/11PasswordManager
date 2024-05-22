using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUi3Test.Datatypes;
using WinUi3Test.Datatypes.Serializing;

namespace WinUi3Test.Util;

public class DialogOperation : Operation
{
    public DialogOperation(XamlRoot xamlRoot, string title, string primaryText, string secondaryText = null, string content = null)
    {
        var dialog = new ContentDialog();
        //var Parameters = new DialogParameters(title, content, primaryText, secondaryText);
        dialog.XamlRoot = xamlRoot;
        dialog.Title = title;
        dialog.Content = content;
        dialog.PrimaryButtonText = primaryText;
        dialog.SecondaryButtonText = secondaryText;
        dialog.DefaultButton = ContentDialogButton.Primary;
        //dialog.PrimaryButtonStyle = Application.Current.Resources["Acce"] as Style;
        dialog.PrimaryButtonClick += (_, _) => this.Finish(true);
        dialog.SecondaryButtonClick += (_, _) => this.Finish(false);
        dialog.ShowAsync();
    }
}
