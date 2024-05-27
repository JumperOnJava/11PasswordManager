using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.src.Util;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Password11.GUI.Dialogs;

public sealed partial class PasswordInputDialog : Page
{
    public PasswordInputDialogModel model = new();

    public PasswordInputDialog(ContentDialog dialog, bool hasSecondField)
    {
        model.HasSecondField = hasSecondField;
        model.PropertyChanged += (_, _) =>
        {
            dialog.IsPrimaryButtonEnabled = model.SamePasswordTextVisible == Visibility.Collapsed &&
                                            !string.IsNullOrEmpty(model.Password);
        };
        model.onPropertyChanged();
        InitializeComponent();
    }

    public static Operation<string> AskPassword(Page parent, bool hasSecondField, string title = "Enter password")
    {
        var dialog = new ContentDialog();
        dialog.Title = title;
        dialog.PrimaryButtonText = "Confirm";
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.SecondaryButtonText = "Cancel";
        dialog.XamlRoot = parent.XamlRoot;
        dialog.Content = new PasswordInputDialog(dialog, hasSecondField);
        var op = new Operation<string>();
        parent.DispatcherQueue.TryEnqueue(async () =>
        {
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                op.FinishSuccess(((PasswordInputDialog)dialog.Content).model.Password);
        });
        return op;
    }

    public class PasswordInputDialogModel : PropertyChangable
    {
        public bool HasSecondField;
        private string password;
        public string passwordRepeat;

        public string Password
        {
            get => password;
            set
            {
                password = value;
                onPropertyChanged();
                onPropertyChanged(nameof(SamePasswordTextVisible));
            }
        }

        public string PasswordRepeat
        {
            get => passwordRepeat;
            set
            {
                passwordRepeat = value;
                onPropertyChanged();
                onPropertyChanged(nameof(SamePasswordTextVisible));
            }
        }

        public Visibility isDoubleInput => HasSecondField ? Visibility.Visible : Visibility.Collapsed;

        public Visibility SamePasswordTextVisible
        {
            get
            {
                if (!HasSecondField)
                    return Visibility.Collapsed;
                if (password == passwordRepeat)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }
    }
}