using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Dialogs;
using Password11.StorageManager;

namespace Password11.GUI.Dialogs;

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
        var passwordResult = await PasswordInputDialog.AskPassword(page, true, "Enter password").GetResult();
        if (!passwordResult.Item1)
        {
            FinishFail();
            return;
        }

        var password = passwordResult.Item2;
        FinishSuccess(fileManager.AesEncryptedManager(password));
    }
}