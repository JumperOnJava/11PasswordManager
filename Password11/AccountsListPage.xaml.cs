using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Animation;
using Password11.Datatypes;
using Password11.src.Util;
using Password11.src.ViewModel;
using Password11.Util;
using Password11.ViewModel;
using Password11Lib.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Password11
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
            var operation = new Operation<UiTag>(new UiTag(new TagBasic()));
            TagEditDialog.ShowEditDialog(XamlRoot,operation);
            operation.OnResult += (ok, result) =>
            {
                if (ok) model.Tags.Add(result);
            };
        }
        private void CreateAccount(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            var newAccount = AccountEditor.Start(Model.RawTags);
            var screen = new AccountCreateDialog(dialog,newAccount,new List<Tag>(model.Tags.Select(uitag => uitag.Target)));
            newAccount.OnFinished += (account) =>
            {
                if (account != null)
                    model.Accounts.Add(new UiAccount(account));
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
                var tag = (UiTag)control.CommandParameter;
                var operation = new Operation<UiTag>(tag);
                
                TagEditDialog.ShowEditDialog(this.XamlRoot, operation);
                operation.OnResult += (ok,result) =>
                {
                    if (!ok) return;
                    var index = model.Tags.IndexOf(tag);
                    model.Tags[index] = result;
                    tag.onPropertyChanged("Target");
                };
            }
        }

        private void AccountOnDragStarting(object o, DragItemsStartingEventArgs drag)
        {
            AccountDeleteButton.Visibility = Visibility.Visible;
            var item = drag.Items[0] as Identifiable<Account>;
            if(item==null)
                return;
            Console.WriteLine(item.Identifier.id.ToString());
            drag.Data.SetText(item.Identifier.id.ToString());
        }

        private void AccountOnDragOver(ListViewBase listViewBase, DragItemsCompletedEventArgs drag)
        {
            AccountDeleteButton.Visibility = Visibility.Collapsed;
        }
        private void TagOnDragStarting(object o, DragItemsStartingEventArgs drag)
        {
            TagDeleteButton.Visibility = Visibility.Visible;
            var item = drag.Items[0] as Identifiable<Tag>;
            if (item == null)
                return;
            Console.WriteLine(item.Identifier.id.ToString());
            drag.Data.SetText(item.Identifier.id.ToString());
        }

        private void TagOnDragOver(ListViewBase listViewBase, DragItemsCompletedEventArgs drag)
        {
            TagDeleteButton.Visibility = Visibility.Collapsed;
        }

        private async void DeleteButton_OnDrop(object sender, DragEventArgs e) 
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                var result = await e.DataView.GetTextAsync();
                Console.WriteLine(result);
                long id = long.Parse(await e.DataView.GetTextAsync());
                model.Accounts.ToList().ForEach(e=>Console.WriteLine("list>"+e.Identifier.id));
                foreach (var account in model.Accounts)
                {
                    if (account.Identifier.id == id)
                    {
                        var dialogOperation = new AskDialogOperation(this,"Delete Account","Delete","Cancel","Are you sure you want to delete account?");
                        dialogOperation.OnFinished += (ok) =>
                        {
                            if (ok)
                                model.Accounts.Remove(account);
                        };  
                        break;
                    }
                }
                foreach (var tag in model.Tags)
                {
                    if (tag.Identifier.id == id)
                    {
                        var dialogOperation = new AskDialogOperation(this, "Delete tag", "Delete", "Cancel", "Are you sure you want to delete this tag?");
                        dialogOperation.OnFinished += (ok) =>
                        {
                            if (ok)
                                model.Tags.Remove(tag);
                        };
                        break;
                    }
                }
                model.Save();
            }
        }

        private async void DeleteButton_OnDragEnter(object sender, DragEventArgs e)
        {
            long data=-1;
            e.AcceptedOperation =  e.DataView.Contains(StandardDataFormats.Text) && long.TryParse(await e.DataView.GetTextAsync(), out data) ? DataPackageOperation.Move : DataPackageOperation.None;
            e.DragUIOverride.Caption = "Delete element";
            e.DragUIOverride.IsGlyphVisible = false;
            
            Console.WriteLine($"Received tag: {await e.DataView.GetTextAsync()}");
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            var button = (ButtonBase)sender;
            var data = new DataPackage();
            data.SetText((string)button.CommandParameter);
            Clipboard.SetContent(data);

        }

        private void StartEditAccount(object sender, RoutedEventArgs e)
        {
            var uiAccount = (UiAccount)((ButtonBase)sender).CommandParameter;
            var operation = AccountEditor.Start(uiAccount.Target,model.RawTags);
            operation.OnFinished += result =>
            {
                if (result != null)
                {
                    uiAccount.Target.Restore(result);
                    uiAccount.onPropertyChanged("Target");
                    model.Save();
                }
                model.Navigator.GoBack();
            };
            model.Navigator.Navigate(uiAccount.Target.AccountEditor, operation, new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void InvertVisibility(object sender, RoutedEventArgs e)
        {
            var uiAccount = (UiAccount)((ButtonBase)sender).CommandParameter;
            uiAccount.CopyMenuVisible = !uiAccount.CopyMenuVisible;
        }
    }
}
