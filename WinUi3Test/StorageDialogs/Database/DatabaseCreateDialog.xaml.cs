using Microsoft.UI.Xaml.Controls;
using System;
using WinUi3Test.Datatypes;
using WinUi3Test.src.Util;

namespace WinUi3Test.StorageDialogs.Database
{
    public sealed partial class DatabaseCreateDialog : Page, DialogPage
    {
        private readonly EmptyOperation<StorageManager> operation;

        public DatabaseCreateDialog(EmptyOperation<StorageManager> operation)
        {
            this.operation = operation;
            this.InitializeComponent();
        }

        public event Action onClose;
        public ContentDialog Dialog { get; set; }
        public void Cancel()
        {
            operation.Finish(false);
        }
    }
}
