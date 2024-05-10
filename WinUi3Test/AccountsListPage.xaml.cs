using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Appointments.DataProvider;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;
using WinUi3Test.src.ViewModel;
using WinUi3Test.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test
{
    public sealed partial class AccountsListPage : Page
    {
        public static MainWindowModel Model;
        public MainWindowModel model
        {
            get => Model;
            set => Model = value;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                model = e.Parameter as MainWindowModel;
                model.Accounts.CollectionChanged += (_,_) => Refilter();
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException("Passed wrong class as page parameter", ex);
            }
        }
        public AccountsListPage()
        {
            this.InitializeComponent();
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(CheckBox))
            {
                var checkbox = sender as CheckBox;
                (checkbox.CommandParameter as UiTag).Selected = (bool)checkbox.IsChecked;
                
                Refilter();
            }
        }


        private async void CreateTag(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            var newTag = new UiTag(new TagBasic("").Identifier);
            var result = await TagEditDialog.ShowEditDialog(this.XamlRoot,newTag);
            if(result == ContentDialogResult.Primary)
            {
                model.Tags.Add(newTag);
            }
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            foreach (var tag in model.Tags)
            {
                tag.Selected = false;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void CreateAccount(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            var newAccount = AccountOperation.Start();
            var screen = new AccountCreationScreen(dialog,newAccount,new List<TagRef>(model.Tags.Map(e=>e.Identifier)));
            newAccount.OnFinished += (account) =>
            {
                    if (account != null) 
                        model.Accounts.Add(new UiAccountModel(model.navigator,AccountOperation.Start(account)));
            };
        }
        private void ShowPane_Click(object sender, RoutedEventArgs e)
        {
            model.IsPaneOpen = true;
        }
        private void Refilter()
        {
            model.FilterAccounts();
        }

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            model.Save();
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            model.ExitNoSave();
        }

    }
}
