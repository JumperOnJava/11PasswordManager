using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Dialogs;
using Password11.src.Util;
using Password11.StorageDialogs.FileStorage;

namespace Password11.StorageDialogs.GlobalCreate;

internal class FileCreateDialogManager : DialogManager
{
    private readonly EmptyOperation<StorageManager> operation;

    public FileCreateDialogManager(EmptyOperation<StorageManager> managerOperation)
    {
        operation = managerOperation;
    }

    public async void Start(Page page)
    {
        var fileResult = await new FileCreateDialog().GetResult();
        if (!fileResult.Item1)
        {
            operation.FinishFail();
            return;
        }

        var fileManager = fileResult.Item2;
        var passwordResult = await PasswordDialog.AskPassword(page, true).GetResult();
        if (!passwordResult.Item1)
        {
            operation.FinishFail();
            return;
        }

        var password = passwordResult.Item2;
        operation.FinishSuccess(fileManager.AesEncryptedManager(password));
    }
}