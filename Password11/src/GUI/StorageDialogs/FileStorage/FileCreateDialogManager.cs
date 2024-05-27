using Microsoft.UI.Xaml.Controls;
using Password11.Dialogs;
using Password11.GUI.StorageDialogs.FileStorage;
using Password11.GUI.StorageDialogs.GlobalCreate;
using Password11.src.Util;

namespace Password11.StorageDialogs.GlobalCreate;

internal class FileCreateDialogManager : DialogManager
{
    public override async void Start(Page page)
    {
        var fileResult = await new FileCreateDialog().GetResult();
        if (!fileResult.Item1)
        {
            FinishFail();
            return;
        }

        var fileManager = fileResult.Item2;
        var passwordResult = await PasswordDialog.AskPassword(page, true).GetResult();
        if (!passwordResult.Item1)
        {
            FinishFail();
            return;
        }

        var password = passwordResult.Item2;
        FinishSuccess(fileManager.AesEncryptedManager(password));
    }
}