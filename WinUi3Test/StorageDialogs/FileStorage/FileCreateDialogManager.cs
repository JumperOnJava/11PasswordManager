using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUi3Test.Datatypes;
using WinUi3Test.Dialogs;
using WinUi3Test.src.Util;
using WinUi3Test.StorageDialogs.FileStorage;

namespace WinUi3Test.StorageDialogs.GlobalCreate;

internal class FileCreateDialogManager : DialogManager
{
    private readonly EmptyOperation<StorageManager> operation;

    public FileCreateDialogManager(EmptyOperation<StorageManager> managerOperation)
    {
        this.operation = managerOperation;
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