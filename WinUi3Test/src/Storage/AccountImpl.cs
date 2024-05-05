using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.Storage
{
    public class AccountImpl : PropertyChangable, Account
    {
        public string targetApp;
        public string displayName;
        public string username;
        public string email;
        public string password;
        public ColorsScheme colors;
        public ObservableCollection<Tag> Tags { get; }
        public long Identifier { get; }

        public string TargetApp { get => targetApp; set { targetApp = value; onPropertyChanged("TargetApp"); } }
        public string DisplayName { get => displayName; set { displayName = value; onPropertyChanged("DisplayName"); } }
        public string Username { get => username; set { username = value; onPropertyChanged("Username"); } }
        public string Email { get => email; set { email = value; onPropertyChanged("Email"); } }
        public string Password { get => password; set { password = value; onPropertyChanged("Password"); } }
        public ColorsScheme Colors
        {
            get => colors; set
            {
                colors = value;
                onPropertyChanged("Colors");
                onPropertyChanged("BaseColorBindable");
                onPropertyChanged("BaseColorBrush");
            }
        }
        public AccountImpl(string targetApp, string username, string password) : this(targetApp, username, password, new List<Tag>()) { }
        public AccountImpl(string targetApp, string username, string password, IList<Tag> tags)
        {
            TargetApp = targetApp.Clone() as String;
            Username = username.Clone() as String;
            Password = password.Clone() as String;
            DisplayName = "";
            Email = "";
            Tags = new ObservableCollection<Tag>(tags);
            Tags.CollectionChanged += (_, _) => onPropertyChanged("Tags");
            Identifier = Random.Shared.NextInt64();
            Colors = ColorsScheme.AccentColors;
        }
        public AccountImpl() : this("", "", "") { }
        public Account Clone()
        {
            var newAccount = new AccountImpl(TargetApp, Username, Password, Tags);
            newAccount.DisplayName = DisplayName.Clone() as String;
            newAccount.Email = Email.Clone() as String;
            newAccount.Colors = Colors;
            return newAccount;
        }
        public Color BaseColorBindable
        {
            get
            {
                return Colors.BaseColor.asWinColor;
            }
            set
            {
                Colors = new ColorsScheme(new AdvColor(value));
            }
        }
    }
}