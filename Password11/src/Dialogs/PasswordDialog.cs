using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.GUI.StorageDialogs.Password;

namespace Password11.Dialogs;

public class PasswordDialog
{
    public static Operation<string> AskPassword(Page parent, bool hasSecondField, string title = "Enter password")
    {
        return PasswordInputDialog.AskPassword(parent, hasSecondField, title);
    }
}