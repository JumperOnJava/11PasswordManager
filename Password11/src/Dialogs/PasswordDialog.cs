using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;

namespace Password11.Dialogs;

public class PasswordDialog
{
    
    public static EmptyOperation<string> AskPassword(Page parent,bool hasSecondField)
    {
        var dialog = new ContentDialog();
        dialog.Title = "Enter password:";
        dialog.PrimaryButtonText = "Confirm";
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.SecondaryButtonText = "Cancel";
        dialog.XamlRoot = parent.XamlRoot;
        dialog.Content = new PasswordInputDialog(dialog,hasSecondField);
        var op = new EmptyOperation<string>();
        parent.DispatcherQueue.TryEnqueue(async () =>
        {
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                op.FinishSuccess(((PasswordInputDialog)dialog.Content).model.Password);
            }
        });
        return op;
    }
}