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
    public sealed partial class OpenStorageDialog : Page, DialogPage
    {
        private EmptyOperation<DialogPage> operation;
        private EmptyOperation<StorageManager> managerOperation;


        public OpenStorageDialog(EmptyOperation<DialogPage> a, EmptyOperation<StorageManager> b)
        {
            this.operation = a;
            this.managerOperation = b;
            this.InitializeComponent();
            operation.OnFinished += (_) => onClose.Invoke();
        }
        private void StartDatabase(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            //operation.FinishSuccess(new FileCreateDialog(new EmptyOperation<StorageManager>()));
        }

        private void StartFile(object sender, RoutedEventArgs e)
        {
            operation.FinishSuccess(new FileOpenDialog(managerOperation));
        }

        public event Action onClose;
    }
}
