using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Dialogs;
using Password11.src.Datatypes.Storage;
using Password11.src.Util;
using Password11.StorageDialogs.Database;
using Password11.Util;

namespace Password11.StorageDialogs.GlobalCreate;

internal class DatabaseDialogManager : DialogManager
{
    private readonly EmptyOperation<StorageManager> operation;
    private readonly Variant variant;

    private DatabaseDialogManager(EmptyOperation<StorageManager> managerOperation, Variant variant)
    {
        operation = managerOperation;
        this.variant = variant;
    }

    private enum Variant
    {
        Register,
        Open,
    }

    public static DatabaseDialogManager CreateOpenManager(EmptyOperation<StorageManager> managerOperation)
    {
        return new DatabaseDialogManager(managerOperation, Variant.Open);
    }
    public static DatabaseDialogManager CreateRegisterManager(EmptyOperation<StorageManager> managerOperation)
    {
        return new DatabaseDialogManager(managerOperation, Variant.Register);
    }


    public async void Start(Page parent)
    {
        var loginDataOpearation = new EmptyOperation<Tuple<string, string, string, string>>();
        var screen = new DatabaseSetupDialog(loginDataOpearation, variant == Variant.Register);
        DialogCreator.StartDialog(screen, parent);
        var loginResult = await loginDataOpearation.GetResult();
        var success = loginResult.Item1;
        var tuple = loginResult.Item2;
        if (!success)
            return;
        var host = tuple.Item1;
        var login = tuple.Item2;
        var password = tuple.Item3;
        var key = tuple.Item4;
        var task = variant 
                   == Variant.Open 
                    ? DatabaseStorageManager.OpenWithConnectionCheck(host, login, password) 
                    : DatabaseStorageManager.RegisterWithConnectionCheck(host, login, password);
        await ExceptionDialog.ShowExceptionOnFail(parent, () =>
        {
            try
            {
                task.Wait();
            }
            catch (Exception e)
            {
                operation.FinishFail();
                throw;
            }
            operation.FinishSuccess(task.Result.AesEncryptedManager(key));
        });
        operation.FinishFail();
    }
}