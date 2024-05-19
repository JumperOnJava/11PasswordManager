using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using WinUi3Test.Datatypes;
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
            dialog.PrimaryButtonClick += (_,_) => tag.Finish(true);
            dialog.PrimaryButtonClick += (_,_) => tag.Finish(false);
            dialog.ShowAsync();
        }
        public TagEditDialog(ContentDialog dialog, Operation<UiTag> tag)
        {
            InitializeComponent();
            this.dialog = dialog;
            this.target = tag.target;
            ColorPickerRing.Color = ColorsScheme.AccentColors.BaseColor.asWinColor;
            Action<string> buttonEnabled = (s) =>
            {
                dialog.IsPrimaryButtonEnabled = s != string.Empty;
            };
            target.TextChanged += buttonEnabled;
            buttonEnabled(tag.target.DisplayName);
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
