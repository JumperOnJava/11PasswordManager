using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Password11.Datatypes;
using Password11.src.Util;
using Password11.StorageDialogs.Database;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Password11.StorageDialogs.GlobalCreate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateStorageDialog : Page, DialogPage
    {
        private Operation<DialogManager> operation;
        private readonly EmptyOperation<StorageManager> managerOperation;

        public ContentDialog Dialog { set { } }

        public event Action onClose;
        public void Cancel()
        {
            operation.Finish(false);
        }

        public CreateStorageDialog(Operation<DialogManager> operation,EmptyOperation<StorageManager> managerOperation)
        {
            InitializeComponent();
            this.operation = operation;
            this.managerOperation = managerOperation;
            operation.OnFinished += _ => onClose.Invoke();
        }

        private void StartDatabase(object sender, RoutedEventArgs e)
        {
            operation.FinishSuccess(DatabaseDialogManager.CreateRegisterManager(managerOperation));
        }
        private void StartFile(object sender, RoutedEventArgs e)
        {
            operation.FinishSuccess(new FileCreateDialogManager(managerOperation));
        }
        private void OpenDatabase(object sender, RoutedEventArgs e)
        {
            operation.FinishSuccess(DatabaseDialogManager.CreateOpenManager(managerOperation));
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            operation.FinishSuccess(new FileOpenDialogManager(managerOperation));
        }

        public static void CreateManager(Page page,Action<StorageManager> receiveMethod)
        {
            var dialogCreator = new EmptyOperation<DialogManager>();
            var managerCreator = new EmptyOperation<StorageManager>();
            dialogCreator.OnResult += (success, result) =>
            {
                if (success)
                {
                    result.Start(page);
                }
            };
            managerCreator.OnResult += (success, result) =>
            {
                if (success)
                {
                    receiveMethod(result);
                }
            };
            new CreateStorageDialog(dialogCreator, managerCreator).StartDialog(page);
        }
    }

    public interface DialogManager
    {
        void Start(Page parent);
    }
}
