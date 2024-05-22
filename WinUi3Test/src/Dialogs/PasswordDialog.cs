using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUi3Test.Dialogs;

public class PasswordDialog
{
    
    public static async Task<string> AskPassword(XamlRoot xamlRoot,bool hasSecondField)
    {
        var dialog = new ContentDialog();
        dialog.Title = "Enter password:";
        dialog.PrimaryButtonText = "Confirm";
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.SecondaryButtonText = "Cancel";
        dialog.XamlRoot = xamlRoot;
        dialog.Content = new PasswordInputDialog(dialog,hasSecondField);
        var result = await dialog.ShowAsync();
        if(result == ContentDialogResult.Primary)
        {
            return ((PasswordInputDialog)dialog.Content).model.Password;
        }
        return null;
    }
}