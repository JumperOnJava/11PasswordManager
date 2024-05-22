using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public event Action onClose;

        public CreateStorageDialog(Operation<DialogPage> operation, Operation<StorageManager> managerOperation)
        {
            this.operation = operation;
            operation.OnFinished += (_) => onClose.Invoke();
        }

        private void StartDatabase(object sender, RoutedEventArgs e)
        {
            operation.FinishSuccess(new FileCreateDialog(new Operation<StorageManager>()));
        }

        private void StartFile(object sender, RoutedEventArgs e)
        {
            operation.FinishSuccess(new FileCreateDialog(new Operation<StorageManager>()));
        }
    }
}
