using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using WinUi3Test.Datatypes;
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


        private void CreateTag(object sender, RoutedEventArgs e)
        {
            var operation = new Operation<UiTag>(new UiTag(new TagBasic().IdentifierTagId));
            TagEditDialog.ShowEditDialog(XamlRoot,operation);
            operation.OnResult += (ok, result) =>
            {
                if (ok) model.Tags.Add(result);
            };
        }

        private void ClearSelection(object sender, RoutedEventArgs e)
        {
            foreach (var tag in model.Tags)
            {
                tag.Selected = false;
            }
        }

        private void CreateAccount(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            var newAccount = AccountOperation.Start();
            var select = model.Tags.Select(e => new UniqueTagId(e.Identifier.id));
            var screen = new AccountCreationScreen(dialog,newAccount,new List<UniqueTagId>(select));
            newAccount.OnFinished += (account) =>
            {
                    if (account != null) 
                        model.Accounts.Add(new UiAccount(model.navigator,AccountOperation.Start(account)));
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

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            model.Exit();
        }

        private void EditTag(object sender, RoutedEventArgs e)
        {
            if (sender is Button control)
            {
                var tag = control.CommandParameter as UiTag;
                var operation = new Operation<UiTag>(tag);
                
                TagEditDialog.ShowEditDialog(this.XamlRoot, operation);
                operation.OnResult += (ok,result) =>
                {
                    if (!ok) return;
                    var index = model.Tags.IndexOf(tag);
                    model.Tags[index] = result;
                };
            }
        }

        private void ElementOnDragStarting(object o, DragItemsStartingEventArgs drag)
        {
            DeleteButton.Visibility = Visibility.Visible;
            var item = drag.Items[0] as Identifiable;
            if(item==null)
                return;
            Console.WriteLine(item.Identifier.id.ToString());
            drag.Data.SetText(item.Identifier.id.ToString());
        }

        private void ElementOnDragOver(ListViewBase listViewBase, DragItemsCompletedEventArgs drag)
        {
            DeleteButton.Visibility = Visibility.Collapsed;
        }

        private void DeleteButton_OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                Console.WriteLine(e.DataView.GetTextAsync().GetResults());
                
            }
        }

        private void DeleteButton_OnDragEnter(object sender, DragEventArgs e)
        {
            long data=-1;
            e.AcceptedOperation =  e.DataView.Contains(StandardDataFormats.Text) && long.TryParse(e.DataView.GetTextAsync().GetResults(), out data) ? DataPackageOperation.Move : DataPackageOperation.None;
            e.DragUIOverride.Caption = "Delete element";
            e.DragUIOverride.IsGlyphVisible = false;
            
            Console.WriteLine($"Received tag: {e.DataView.GetTextAsync().GetResults()}");
            
        }
    }
}
