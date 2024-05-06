using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.ViewModel
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
        private Account target;
        public static readonly DependencyProperty BaseColorBrushProperty = DependencyProperty.Register(nameof(BaseColorBrush), typeof(object), typeof(AccountOperation), new PropertyMetadata(default(object)));
        public event Action<Account?> onFinished;

        private AccountOperation(Account target)
        {
            this.target = target.Clone();
            onPropertyChanged("target");
            this.target.PropertyChanged += (s, e) =>
            {
                onPropertyChanged(e.PropertyName);
            };
        }

        public void Finish(bool successful)
        {   
            onFinished.Invoke(successful ? target : null);
        }
        public string TargetApp
        {
            get => target.TargetApp; set
            {
                target.TargetApp = value;
            }
        }
        public string DisplayName
        {
            get => target.DisplayName; set
            {
                target.DisplayName = value;
            }
        }
        public string Username
        {
            get => target.Username; set
            {
                target.Username = value;
            }
        }
        public string Email
        {
            get => target.Email; set
            {
                target.Email = value;
            }
        }
        public string Password
        {
            get => target.Password; set
            {
                target.Password = value;
            }
        }
        public ColorsScheme Colors
        {
            get => target.Colors; set
            {
                target.Colors = value;
            }
        }
        public long Identifier => target.Identifier;
        public ObservableCollection<Tag> Tags => target.Tags;

        public Color BaseColorBindable => target.BaseColorBindable;

        Color Account.BaseColorBindable { get => target.BaseColorBindable; set => target.BaseColorBindable = value; }

        public Brush BaseColorBrush => Colors.BaseColor.asBrush;
        public Brush HoverColorBrush => Colors.HoverColor.asBrush;
        public Brush SymbolColoBrush => Colors.SymbolColor.asBrush;

        public Account Clone()
        {
            return target.Clone();
        }

        public void Write(JsonObject dataWriter)
        {
            throw new NotSupportedException("AccountOperation is not supposed to be serialized");
        }

        public object Read(JsonObject element)
        {
            throw new NotSupportedException("AccountOperation is not supposed to be serialized");
        }
    }
}
