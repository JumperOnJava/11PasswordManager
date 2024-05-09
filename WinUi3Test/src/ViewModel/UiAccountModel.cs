using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Isolation;
using WinUi3Test.src.Storage;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;
using WinUi3Test.ViewModel;

namespace WinUi3Test.src.ViewModel
{
    public class UiAccountModel : PropertyChangable
    {
        protected Frame navigator;
        private AccountOperation target;
        public AccountOperation Target
        {
            get => target; set
            {
                target = value;
                onPropertyChanged();
            }
        }

        public UiAccountModel(Frame navigator, AccountOperation target) 
        {
            this.navigator = navigator;
            this.Target = target;
        }

        private bool copyMenuVisible;
        public bool CopyMenuVisible { 
            get => copyMenuVisible;
            set {
                this.copyMenuVisible = value;
                onPropertyChanged("CopyMenuVisible");
            }
        }
        public Action InvertVisibility_ => InvertVisibility;
        public void InvertVisibility() => CopyMenuVisible = !CopyMenuVisible;

        public Action EditThisAccount_ => EditThisAccount;
        public void EditThisAccount() => Navigate();
        public bool EmailVisible => target.Email.Replace(" ", "").Length > 0;
        public bool UsernameVisible => target.Username.Replace(" ", "").Length > 0;
        public bool AppLinkBottonVisible => target.AppLink.Replace(" ", "").Length > 0;

        public void CallEntryMethod(object sender, RoutedEventArgs args)
        {
            if(sender is ButtonBase)
            {
                
                ((sender as ButtonBase).CommandParameter as Action).Invoke();
            }
        }
        private Frame Navigator { get; set; }
        public void Navigate()
        {
            var operation = AccountOperation.Start(Target.target);
            operation.OnFinished += result =>
            {
                if (result != null)
                {
                    this.Target.target = result;
                }
                target.Finish(true);
                navigator.GoBack();
            };
            navigator.Navigate(this.Target.target.AccountEditor, operation, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }
}