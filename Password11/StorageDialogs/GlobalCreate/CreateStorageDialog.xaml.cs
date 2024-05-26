using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.src.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Password11.StorageDialogs.GlobalCreate;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CreateStorageDialog : Page, DialogPage
{
    private readonly Operation<DialogManager> operation;

    public CreateStorageDialog(Operation<DialogManager> operation, Operation<StorageManager> managerOperation)
    {
        InitializeComponent();
        this.operation = operation;
        operation.OnFinished += _ => onClose.Invoke();
    }

    public ContentDialog Dialog
    {
        set { }
    }

    public event Action onClose;

    public void Cancel()
    {
        operation.Finish(false);
    }

    private void StartDatabase(object sender, RoutedEventArgs e)
    {
        operation.FinishSuccess(DatabaseDialogManager.CreateRegisterManager());
    }

    private void StartFile(object sender, RoutedEventArgs e)
    {
        operation.FinishSuccess(new FileCreateDialogManager());
    }

    private void OpenDatabase(object sender, RoutedEventArgs e)
    {
        operation.FinishSuccess(DatabaseDialogManager.CreateOpenManager());
    }

    private void OpenFile(object sender, RoutedEventArgs e)
    {
        operation.FinishSuccess(new FileOpenDialogManager());
    }

    public static void CreateManager(Page page, Action<StorageManager> receiveMethod)
    {
        var dialogCreator = new Operation<DialogManager>();
        var managerCreator = new Operation<StorageManager>();
        dialogCreator.OnResult += (success, result) =>
        {
            if (success) result.Start(page);
        };
        managerCreator.OnResult += (success, result) =>
        {
            if (success) receiveMethod(result);
        };
        new CreateStorageDialog(dialogCreator, managerCreator).StartDialog(page);
    }
}

public abstract class DialogManager : Operation<StorageManager>
{
    public abstract void Start(Page parent);
}