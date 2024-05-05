    using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;
using WinUi3Test.src.ViewModel;

namespace WinUi3Test
{
    public sealed partial class PasswordEdit : Page
    {
        private PasswordEditModel model;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                model.Target = e.Parameter as AccountOperation;
                ColorPickerRing.Color = model.Target.Colors.BaseColor.asWinColor;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException("Passed wrong class as page parameter", ex);
            }
        }
        public PasswordEdit()
        {
            model = new PasswordEditModel();
            InitializeComponent();
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Title = "Save changes?";
            dialog.PrimaryButtonText = "Save";
            dialog.SecondaryButtonText = "Cancel";

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                model.Target.Finish(true);
            }
        }
        private async void Cancel(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Title = "Cancel changes?";
            dialog.PrimaryButtonText = "Confirm";
            dialog.SecondaryButtonText = "Cancel";

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                model.Target.Finish(false);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            model.PasswordRevealMode = ((bool)PasswordCheck.IsChecked) ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden; 
        }

        private void ColorPickerRing_OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            model.Target.Colors = new ColorsScheme(new AdvColor(sender.Color));
        }
    }

    internal class PasswordEditModel : PropertyChangable
    {
        private AccountOperation target;

        private PasswordRevealMode passwordRevealMode;
        public PasswordRevealMode PasswordRevealMode
        {
            get => passwordRevealMode; set
            {
                passwordRevealMode = value;
                onPropertyChanged("PasswordRevealMode");
            }
        }
        public AccountOperation Target
        {
            get => target; set
            {
                target = value;
                onPropertyChanged("Target");
            }
        }
    }
}
