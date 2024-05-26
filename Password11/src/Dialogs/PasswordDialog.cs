using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;

namespace Password11.Dialogs;

public class PasswordDialog
{
    public static Operation<string> AskPassword(Page parent, bool hasSecondField, string title = "Enter password")
    {
       return  PasswordInputDialog.AskPassword(parent, hasSecondField, title);
    }
}