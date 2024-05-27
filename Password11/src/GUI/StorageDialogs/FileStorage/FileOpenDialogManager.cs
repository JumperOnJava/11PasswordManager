using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Password11.Dialogs;
using Password11.GUI.StorageDialogs.GlobalCreate;
using Password11.src.Util;
using Password11.StorageDialogs.FileStorage;

namespace Password11.StorageDialogs.GlobalCreate;

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
        var passwordResult = await PasswordDialog.AskPassword(parent, false).GetResult();

        if (!passwordResult.Item1) return;

        var password = passwordResult.Item2;
        FinishSuccess(result.AesEncryptedManager(password));
    }
}