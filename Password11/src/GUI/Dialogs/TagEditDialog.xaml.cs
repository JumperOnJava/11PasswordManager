using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.ColorLib;
using Password11.Datatypes;
using Password11.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Password11.GUI.Dialogs;

/// <summary>
///     An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TagEditDialog : Page
{
    private readonly UiTag target;

    public TagEditDialog(ContentDialog dialog, Operation<UiTag> tag)
    {
        InitializeComponent();
        target = tag.Target;
        ColorPickerRing.Color = target.TagColors.BaseColor.AsWinColor;
        Action<string> buttonEnabled = s => { dialog.IsPrimaryButtonEnabled = s != string.Empty; };
        target.TextChanged += buttonEnabled;
        buttonEnabled(tag.Target.DisplayName);
    }

    public static void ShowEditDialog(XamlRoot parent, Operation<UiTag> tag)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = parent,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Edit Tag",
            PrimaryButtonText = "Save",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };
        var editDialog = new TagEditDialog(dialog, tag);
        dialog.Content = editDialog;
        dialog.PrimaryButtonClick += (_, _) => tag.Finish(true);
        dialog.SecondaryButtonClick += (_, _) => tag.Finish(false);
        dialog.ShowAsync();
    }

    private void ColorPickerRing_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if (args.OldColor == args.NewColor)
            return;
        var color = args.NewColor;
        target.TagColors = new ColorsScheme(new AdvColor(color.R, color.G, color.B));
    }
}