using System;
using System.Collections.Generic;

namespace WinUi3Test.src.Storage
{
    public class AccountImpl : Account
    {
        public string TargetApp { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Tag> Tags { get; }

        public long Identifier { get; }

        IList<Tag> Taggable.Tags => Tags;
        public AccountImpl(string targetApp, string username, string password) : this(targetApp, username, password, new List<Tag>()) { }
        public AccountImpl(string targetApp, string username, string password, List<Tag> tags)
        {
            TargetApp = targetApp;
            Username = username;
            Password = password;
            Tags = tags;
            Identifier = Random.Shared.NextInt64();
        }
        public Account Clone()
        {
            return new AccountImpl(TargetApp,Username,Password,new List<Tag>(Tags));
        }
    }
}