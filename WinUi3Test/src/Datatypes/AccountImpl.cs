using System;
using System.Collections.Generic;
using Windows.UI;
using WinUi3Test.src.Ui;

namespace WinUi3Test.src.Storage
{
    public class AccountImpl : Account
    {
        public string targetApp;
        public string displayName;
        public string username;
        public string email;
        public string password;
        public ColorsScheme colors;
        public TagRef identifier;
        public List<TagRef> Tags { get; set; }
        public TagRef Identifier => identifier;

        public string TargetApp { get => targetApp; set => targetApp = value; }
        public string DisplayName { get => displayName; set => displayName = value; }
        public string Username { get => username; set => username = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public ColorsScheme Colors { get => colors; set => colors = value; }
        public AccountImpl(string targetApp, string username, string password) : this(targetApp, username, password, new List<TagRef>()) { }
        public AccountImpl(string targetApp, string username, string password, IEnumerable<TagRef> tags)
        {
            TargetApp = targetApp.Clone() as String;
            Username = username.Clone() as String;
            Password = password.Clone() as String;
            DisplayName = "";
            Email = "";
            Tags = new List<TagRef>(tags);
            identifier = new TagRef(Random.Shared.NextInt64());
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
            get => Colors.BaseColor.asWinColor;
            set => Colors = new ColorsScheme(new AdvColor(value));
        }

    }
}