using System;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.Dialogs;
using Password11.src.Util;
using Password11.StorageManager;
using Password11.Util;
using DatabaseSetupDialog = Password11.GUI.Dialogs.DatabaseSetupDialog;

namespace Password11.GUI.Dialogs;

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
        bool cancelled=false;
        var dialog = new DialogBuilder(parent).Title($"Loading {login} from {host}").SecondaryButtonText("Cancel").AddSecondaryClickAction(_ =>
        {
            cancelled = true;
        }).Build();
        dialog.ShowAsync();
        try
        {
            await task;
            dialog.Hide();
            if (!cancelled)
            {
                FinishSuccess(task.Result.AesEncryptedManager(key));
                return;
            }
        }
        catch (Exception e)
        {
            dialog.Hide();
            if(!cancelled)
                await ExceptionDialog.ShowException(parent, e);
        }
        FinishFail();
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