using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using WinUi3Test.src.Ui;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test.src.ViewModel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TagEditDialog : Page
    {
        public static async Task<ContentDialogResult> ShowEditDialog(XamlRoot parent, UiTag tag)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = parent;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Edit Tag";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            var newTag = tag;
            var editDialog = new TagEditDialog(dialog, newTag);
            dialog.Content = editDialog;
            return await dialog.ShowAsync();
        }
        public TagEditDialog(ContentDialog dialog, UiTag tag)
        {
            InitializeComponent();
            this.dialog = dialog;
            this.target = tag;
            ColorPickerRing.Color = ColorsScheme.AccentColors.BaseColor.asWinColor;
            Action<string> buttonEnabled = (s) =>
            {
                dialog.IsPrimaryButtonEnabled = s != String.Empty;
            };
            target.TextChanged += buttonEnabled;
            buttonEnabled(tag.DisplayName);
        }

        private ContentDialog dialog;
        private UiTag target;

        private void ColorPickerRing_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (args.OldColor == args.NewColor)
                return;
            var color = args.NewColor;
            target.TagColors = new ColorsScheme(new AdvColor(color.R, color.G, color.B));
        }
    }
}
