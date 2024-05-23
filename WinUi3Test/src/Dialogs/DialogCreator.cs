using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUi3Test.StorageDialogs.GlobalCreate;
using WinUi3Test.Util;
using Page = Microsoft.UI.Xaml.Controls.Page;

namespace WinUi3Test.src.Util
{
    static class DialogCreator
    {
        public static void StartDialog(this DialogPage page, XamlRoot xamlRoot, string title = null)
        {
            var dialog = new DialogBuilder(xamlRoot)
                .Content(page)
                .SecondaryButtonText("Cancel")
                .AddSecondaryClickAction(page.Cancel)
                .Build();
            page.Dialog = dialog;
            page.onClose += dialog.Hide;
            dialog.ShowAsync();
        }
    }

    public interface DialogPage
    {
        event Action onClose;
        ContentDialog Dialog { set; }
        void Cancel();
    }
}
