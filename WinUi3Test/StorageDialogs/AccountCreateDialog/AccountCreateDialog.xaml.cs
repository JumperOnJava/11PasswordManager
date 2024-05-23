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
using WinUi3Test.Datatypes;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;
using WinUi3Test.src.ViewModel;
using WinUi3Test.ViewModel;

namespace WinUi3Test
{
    public sealed partial class AccountCreateDialog : Page
    {

        private AccountCreationModel model;
        public AccountCreateDialog(ContentDialog dialog, AccountEditor account, IList<Tag> tags)
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
                account.Tags = new List<Tag>(model.SelectedTags).Select(e=>e.Identifier).ToList();
                account.Finish(true);
                a.Hide();
            };
            dialog.CloseButtonClick += (a, b) =>
            {
                a.Hide();
                account.Finish(false);
            };
            model = new AccountCreationModel(account);
            model.Account.PropertyChanged += (_, _) => UpdateSaveButton();
            model.UnselectedTags = new ObservableCollection<Tag>(tags);
            model.SelectedTags = new ObservableCollection<Tag>();
            ColorPickerRing.Color = model.Account.BaseColorBindable;
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
            Tag r = (Tag)((ButtonBase)sender).CommandParameter;
            try
            {
                var tag = model.SelectedTags.First(e => e == r);
                model.SelectedTags.Remove(tag);
                model.UnselectedTags.Add(tag);
            }
            catch (Exception ex)
            {
                var tag = model.UnselectedTags.First(e => e == r);
                model.UnselectedTags.Remove(tag);
                model.SelectedTags.Add(tag);
            }
        }
    }

    internal class AccountCreationModel : PropertyChangable
    {
        public AccountEditor Account { get; }
        public ObservableCollection<Tag> SelectedTags { get; set; }
        public ObservableCollection<Tag> UnselectedTags { get; set; }

        public AccountCreationModel(AccountEditor account)
        {
            this.Account = account;
        }
    }
}
