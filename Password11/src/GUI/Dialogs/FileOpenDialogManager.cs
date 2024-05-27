using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Dialogs;
using Password11.StorageManager;

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

        var result = fileResult.Item2;

        var tcs = new TaskCompletionSource<Tuple<bool, string>>();
        var passwordResult = await PasswordInputDialog.AskPassword(parent, false, "Enter password").GetResult();

        if (!passwordResult.Item1) return;

        var password = passwordResult.Item2;
        FinishSuccess(result.AesEncryptedManager(password));
    }
}