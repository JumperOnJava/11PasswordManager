using Microsoft.UI.Xaml.Controls;
using System;
using Password11.Datatypes;
using Password11.src.Util;

namespace Password11.StorageDialogs.Database
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
