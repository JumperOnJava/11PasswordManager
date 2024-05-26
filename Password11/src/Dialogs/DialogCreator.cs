using System;
using Microsoft.UI.Xaml.Controls;
using Password11.Util;
using Page = Microsoft.UI.Xaml.Controls.Page;

namespace Password11.src.Util;

internal static class DialogCreator
{
    public static void StartDialog(this DialogPage dialogPage, Page page, string title = null)
    {
        var dialog = new DialogBuilder(page)
            .Content(dialogPage)
            .SecondaryButtonText("Cancel")
            .AddSecondaryClickAction(_ => dialogPage.Cancel())
            .Build();
        dialogPage.Dialog = dialog;
        dialogPage.onClose += dialog.Hide;
        dialog.ShowAsync();
    }
}

public interface DialogPage
{
    ContentDialog Dialog { set; }
    event Action onClose;
    void Cancel();
}