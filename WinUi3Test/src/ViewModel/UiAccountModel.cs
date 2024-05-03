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

namespace WinUi3Test.src.ViewModel
{
    public class UiAccountModel : PropertyChangable
    {
        protected Frame navigator;
        private Account target;
        public Account Target
        {
            get => target; set
            {
                target = value;
                onPropertyChanged("Target");
            }
        }

        public UiAccountModel(Frame navigator, Account target) 
        {
            this.navigator = navigator;
            this.Target = target;
            Target.PropertyChanged += (a, e) => onPropertyChanged(e.PropertyName);
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
            var operation = AccountOperation.Start(Target);
            navigator.Navigate(Target.AccountEditor, operation, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
            operation.onFinished += (result) =>
            {
                if (result != null)
                {
                    this.Target = result;
                }
                navigator.GoBack();
            };
        }
    }
}