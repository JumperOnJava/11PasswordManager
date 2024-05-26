using System;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Dialogs;
using Password11.src.Datatypes.Storage;
using Password11.src.Util;
using Password11.StorageDialogs.Database;

namespace Password11.StorageDialogs.GlobalCreate;

internal class DatabaseDialogManager : DialogManager
{
    private readonly Variant variant;

    private DatabaseDialogManager(Variant variant)
    {
        this.variant = variant;
    }


    public override async void Start(Page parent)
    {
        var loginDataOpearation = new Operation<Tuple<string, string, string, string>>();
        var screen = new DatabaseSetupDialog(loginDataOpearation, variant == Variant.Register);
        screen.StartDialog(parent);
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
        try
        {
            task.Wait();
            FinishSuccess(task.Result.AesEncryptedManager(key));
        }
        catch (Exception e)
        {
            FinishFail();
            await ExceptionDialog.ShowException(parent, e);
        }
    }

    public static DatabaseDialogManager CreateOpenManager()
    {
        return new DatabaseDialogManager(Variant.Open);
    }

    public static DatabaseDialogManager CreateRegisterManager()
    {
        return new DatabaseDialogManager(Variant.Register);
    }

    private enum Variant
    {
        Register,
        Open
    }
}