using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUi3Test.StorageDialogs.GlobalCreate;

namespace WinUi3Test.src.Util
{
    static class DialogCreator
    {
        public static void StartDialog(this DialogPage page, XamlRoot xamlRoot, string title = null)
        {
            var dialog = new ContentDialog();
            dialog.Content = page;
            dialog.XamlRoot = xamlRoot;
            page.onClose += dialog.Hide;
            dialog.ShowAsync();
        }
    }

    public interface DialogPage
    {
        event Action onClose;
    }
}
