using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUi3Test.Util;

public class DialogBuilder
{
    private ContentDialog dialog = new();

    public DialogBuilder(XamlRoot root)
    {
        dialog.XamlRoot = root;
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

    public DialogBuilder AddPrimaryClickAction(Action handler)
    {
        dialog.PrimaryButtonClick += (_,_) => handler();
        return this;
    }

    public DialogBuilder AddSecondaryClickAction(Action handler)
    {
        dialog.SecondaryButtonClick += (_,_) => handler();
        return this;
    }

    public ContentDialog Build()
    {
        return dialog;
    }
    
}