using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;
using WinUi3Test.src.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountCreationScreen : Page
    {

        private AccountCreationModel model;
        public AccountCreationScreen(ContentDialog dialog, AccountOperation account, IList<Tag> tags)
        {
            dialog.Content = this;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Add account";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            this.dialog = dialog;
            this.InitializeComponent();
            dialog.ShowAsync().GetResults();
            dialog.IsPrimaryButtonEnabled = false;
            dialog.PrimaryButtonClick += (a, b) =>
            {
                a.Hide();
                account.Finish(true);
            };
            dialog.CloseButtonClick += (a, b) =>
            {
                a.Hide();
                account.Finish(false);
            };
            model = new AccountCreationModel(account);
            model.Account.PropertyChanged += (_, _) => UpdateSaveButton();
            model.UnselectedTags = new ObservableCollection<Tag>(tags);
        }

        private void UpdateSaveButton()
        {
            var enabled = model.Account.TargetApp != String.Empty && model.Account.Password != String.Empty && (model.Account.Username != String.Empty || model.Account.Email != String.Empty);
            dialog.IsPrimaryButtonEnabled = enabled;
        }

        private ContentDialog dialog;

        private void ColorPickerRing_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (args.NewColor == args.OldColor)
                return;
            model.Account.Colors = new ColorsScheme(new AdvColor(args.NewColor));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            long id = (long)((sender as ButtonBase).CommandParameter);
            try
            {
                var tag = model.SelectedTags.First(e => e.Identifier == id);
                model.SelectedTags.Remove(tag);
                model.UnselectedTags.Add(tag);
            }
            catch (Exception ex)
            {
                var tag = model.UnselectedTags.First(e => e.Identifier == id);
                model.UnselectedTags.Remove(tag);
                model.SelectedTags.Add(tag);
            }
        }
    }

    internal class AccountCreationModel : PropertyChangable
    {
        public AccountOperation Account { get; }
        public ObservableCollection<Tag> SelectedTags => Account.Tags;
        public ObservableCollection<Tag> UnselectedTags {  get; set; }

        public AccountCreationModel(AccountOperation account)
        {
            this.Account = account;
        }
    }
}
