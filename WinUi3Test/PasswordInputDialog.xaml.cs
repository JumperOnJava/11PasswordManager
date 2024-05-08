using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUi3Test.src.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Test
{
    public sealed partial class PasswordInputDialog : Page
    {
        public PasswordInputDialogModel model = new PasswordInputDialogModel();
        public PasswordInputDialog(ContentDialog dialog, bool isDouble)
        {
            model.isDouble = isDouble;
            model.PropertyChanged += (_, _) =>
            {
                dialog.IsPrimaryButtonEnabled = model.SamePasswordTextVisible == Visibility.Collapsed;
            };
            this.InitializeComponent();
        }
        public class PasswordInputDialogModel : PropertyChangable
        {
            public String password;
            public String Password
            {
                get => password; set
                {
                    password = value;
                    onPropertyChanged();
                    onPropertyChanged(nameof(SamePasswordTextVisible));
                }
            }
            public String passwordrepeat;
            public bool isDouble;

            public String PasswordRepeat
            {
                get => passwordrepeat; set
                {
                    passwordrepeat = value;
                    onPropertyChanged();
                    onPropertyChanged(nameof(SamePasswordTextVisible));
                }
            }
            public Visibility isDoubleInput => isDouble ? Visibility.Visible : Visibility.Collapsed;
            public Visibility SamePasswordTextVisible
            {
                get
                {
                    if(!isDouble)
                        return Visibility.Collapsed;
                    if (password == passwordrepeat)
                        return Visibility.Collapsed;
                    return Visibility.Visible;
                }
            }
        }
    }
}
