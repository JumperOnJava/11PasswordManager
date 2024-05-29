using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Dialogs;
using Password11.StorageManagers;

namespace Password11.GUI.Dialogs;

internal class FileOpenDialogManager : DialogManager
{
    public override async void Start(Page parent)
    {
        var fileResult = await new FileOpenDialog().GetResult();
        if (!fileResult.Item1)
        {
            FinishFail();
            return;
        }

        var fileManager = fileResult.Item2;

        var passwordResult = await PasswordInputDialog.AskPassword(parent, false, "Enter password").GetResult();

        if (!passwordResult.Item1)
        {
            FinishFail();
            return;
        }

        var password = passwordResult.Item2;
        FinishSuccess(fileManager.AesEncryptedManager(password));
    }
}