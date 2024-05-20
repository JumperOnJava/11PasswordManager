using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using WinUi3Test.Datatypes;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.ViewModel
{
    /// <summary>
    /// Takes input account, clones it, and allows to safely do changes
    /// call Finish() function to get return
    /// make sure object is not used after calling Finish, otherwise undefined behaviour can happen
    /// </summary>
    public class AccountOperation : PropertyChangable, Account
    {
        public static AccountOperation Start()
        {
            return Start(new AccountImpl());
        }

        public static AccountOperation Start(Account account)
        {
            return new AccountOperation(account);
        }

        public Account target;

        public static readonly DependencyProperty BaseColorBrushProperty =
            DependencyProperty.Register(nameof(BaseColorBrush), typeof(object), typeof(AccountOperation),
                new PropertyMetadata(default(object)));

        public event Action<Account?> OnFinished;

        private AccountOperation(Account target)
        {
            this.target = target.Clone();
        }

        public void Finish(bool successful)
        {
            if (OnFinished != null)
            OnFinished.Invoke(successful ? target : null);
        }

        public string TargetApp
        {
            get => target.TargetApp;
            set
            {
                target.TargetApp = value;
                onPropertyChanged();
            }
        }

        public string DisplayName
        {
            get => target.DisplayName;
            set
            {
                target.DisplayName = value;
                onPropertyChanged();
            }
        }

        public string Username
        {
            get => target.Username;
            set
            {
                target.Username = value;
                onPropertyChanged();
            }
        }

        public string Email
        {
            get => target.Email;
            set
            {
                target.Email = value;
                onPropertyChanged();
            }
        }

        public string Password
        {
            get => target.Password;
            set
            {
                target.Password = value;
                onPropertyChanged();
            }
        }

        public Dictionary<string, FieldData> AdditionalData
        {
            get => target.AdditionalData;
            set => target.AdditionalData = value;
        }

        public ColorsScheme Colors
        {
            get => target.Colors;
            set
            {
                target.Colors = value;
                onPropertyChanged();
                onPropertyChanged(nameof(BaseColorBindable));
                onPropertyChanged(nameof(BaseColorBrush));
            }
        }

        public List<UniqueTagId> Tags
        {
            get => target.Tags;
            set
            {
                target.Tags = value;
                onPropertyChanged();
            }
        }
        public Color BaseColorBindable
        {
            get => target.BaseColorBindable;
            set
            {
                target.BaseColorBindable = value;
                onPropertyChanged();
            }
        }

        public Brush BaseColorBrush => Colors.BaseColor.asBrush;
        public Brush HoverColorBrush => Colors.HoverColor.asBrush;
        public Brush SymbolColoBrush => Colors.SymbolColor.asBrush;

        public string AppLink
        {
            get => target.AppLink;
            set
            {
                target.AppLink = value;
                onPropertyChanged();
            }
        }

        public UniqueId Identifier => target.Identifier;

        public Account Clone()
        {
            return target.Clone();
        }
    }
}
