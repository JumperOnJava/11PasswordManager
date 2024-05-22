using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using WinUi3Test.Datatypes;
using WinUi3Test.src.Util;
using WinUi3Test.StorageDialogs.FileStorage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test.StorageDialogs.GlobalCreate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateStorageDialog : Page, DialogPage
    {
        private Operation<DialogPage> operation;
        private readonly Operation<StorageManager> managerOperation;
        public event Action onClose;

        public CreateStorageDialog(Operation<DialogPage> operation, Operation<StorageManager> managerOperation)
        {
            this.operation = operation;
            this.managerOperation = managerOperation;
            operation.OnFinished += (_) => onClose.Invoke();
        }

        private void StartDatabase(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            //operation.FinishSuccess(new FileCreateDialog(new EmptyOperation<StorageManager>()));
        }

        private void StartFile(object sender, RoutedEventArgs e)
        {
            operation.FinishSuccess(new FileCreateDialog(managerOperation));
        }
    }
}
