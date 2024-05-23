using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Dialogs;
using Password11.StorageDialogs.FileStorage;
using Password11.src.Util;

namespace Password11.StorageDialogs.GlobalCreate;

internal class FileOpenDialogManager : DialogManager
{
    private readonly EmptyOperation<StorageManager> operation;

    public FileOpenDialogManager(EmptyOperation<StorageManager> managerOperation)
    {
        this.operation = managerOperation;
    }

    public async void Start(Page parent)
    {
        
        var fileResult = await new FileOpenDialog().GetResult();
        if (!fileResult.Item1)
        {
            operation.FinishFail();
            return;
        }

        var result = fileResult.Item2;
        
        var tcs = new TaskCompletionSource<Tuple<bool,string>>();
        var passwordResult = await PasswordDialog.AskPassword(parent, false).GetResult();

        if (!passwordResult.Item1)
        {
            result.Fail();
            return;
        }

        var password = passwordResult.Item2;
        operation.FinishSuccess(result.AesEncryptedManager(password));
    }
}