using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.ViewModel
{
    public interface UiAccount : Identifiable, INotifyPropertyChanged, Taggable
    {
        public string TargetApp { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool CopyMenuVisible { get; set; }
        public ObservableCollection<Tag> Tags { get; set; }
        public Action InvertVisibility_ { get; }
        public void InvertVisibility();
        public Action EditThisAccount_ { get; }
        public void EditThisAccount();
        public void CallEntryMethod(object sender, RoutedEventArgs args);
        public UiAccount Clone();
    }
    public class UiAccountImpl : UiAccount
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void onPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void CallEntryMethod(object sender, RoutedEventArgs e)
        {
            if (sender is ButtonBase bbase)
            {

                (bbase.CommandParameter as Action).Invoke();
            }
        }

        public UiAccount Clone()
        {
            var newAccount = new UiAccountImpl(WindowModel, TargetApp, Username, Password);
            newAccount.Tags = new ObservableCollection<Tag>(Tags);
            return newAccount;
        }

        private void subscribeTags() => Tags.CollectionChanged += (_, _) => { onPropertyChanged("Tags"); };

        public UiAccountImpl(WindowModel windowModel, string targetApp, string username, string password)
        {
            WindowModel = windowModel;
            TargetApp = targetApp;
            Username = username;
            Password = password;
            Tags = new ObservableCollection<Tag>();
            subscribeTags();
        }


        private string targetApp;
        public string TargetApp
        {
            get => targetApp; set
            {
                targetApp = value;
                onPropertyChanged("TargetApp");
            }
        }

        private string username;
        public string Username
        {
            get => username; set
            {
                username = value;
                onPropertyChanged("Username");
            }
        }

        private string password;
        public string Password
        {
            get => password; set
            {
                password = value;
                onPropertyChanged("Password");
            }
        }
        public long Identifier { get; protected set; }

        public ObservableCollection<Tag> Tags { get; set; }

        IList<Tag> Taggable.Tags => Tags;
        


        WindowModel WindowModel { get; }

        bool copyMenuVisible;
        public bool CopyMenuVisible
        {
            get => copyMenuVisible; set
            {
                copyMenuVisible = value;
                onPropertyChanged("CopyMenuVisible");
            }
        }
        public Action InvertVisibility_ => InvertVisibility;
        public void InvertVisibility()
        {
            CopyMenuVisible = !CopyMenuVisible;
        }

        public Action EditThisAccount_ => EditThisAccount;
        public void EditThisAccount()
        {
            WindowModel.CurrentEditAccount = this;
        }

        public void Accept(UiAccount account)
        {
            this.TargetApp = account.TargetApp;
            this.Username = account.Username;
            this.Password = account.Password;
            this.Tags = account.Tags;
        }
    }
}