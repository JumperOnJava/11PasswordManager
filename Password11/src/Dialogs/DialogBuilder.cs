using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Password11.Util;

public class DialogBuilder
{
    private ContentDialog dialog = new();

    public DialogBuilder(Page page)
    {
        dialog.XamlRoot = page.XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
    }
    public DialogBuilder Title(string title)
    {
        dialog.Title = title;
        return this;
    }

    public DialogBuilder Content(object content)
    {
        dialog.Content = content;
        return this;
    }

    public DialogBuilder PrimaryButtonText(string primaryText)
    {
        dialog.PrimaryButtonText = primaryText;
        return this;
    }

    public DialogBuilder SecondaryButtonText(string secondaryText)
    {
        dialog.SecondaryButtonText = secondaryText;
        return this;
    }

    public DialogBuilder DefaultButton(ContentDialogButton defaultButton)
    {
        dialog.DefaultButton = defaultButton;
        return this;
    }

    public DialogBuilder PrimaryButtonStyle(Style style)
    {
        dialog.PrimaryButtonStyle = style;
        return this;
    }

    public DialogBuilder AddPrimaryClickAction(Action<ContentDialog> handler)
    {
        dialog.PrimaryButtonClick += (dialog,_) => handler(dialog);
        return this;
    }

    public DialogBuilder AddSecondaryClickAction(Action<ContentDialog> handler)
    {
        dialog.SecondaryButtonClick += (dialog,_) => handler(dialog);
        return this;
    }

    public ContentDialog Build()
    {
        return dialog;
    }
    
}