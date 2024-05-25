using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.src.Util;

namespace Password11.StorageDialogs.Database
{
    public sealed partial class DatabaseSetupDialog : Page, DialogPage
    {
        private readonly EmptyOperation<Tuple<string,string,string,string>> operation;
        private readonly DatabaseCreateDialogModel Model = new();

        public DatabaseSetupDialog(EmptyOperation<Tuple<string, string, string, string>> operation,
            bool hasSecondField)
        {
            this.operation = operation;
            Model.HasSecondField = hasSecondField;
            this.InitializeComponent();
        }

        public event Action onClose;

        public ContentDialog Dialog
        {
            get => null;
            set
            {
                value.PrimaryButtonText = "Confirm";
                Model.PropertyChanged += (_, _) =>
                {
                    value.IsPrimaryButtonEnabled = Model.SamePasswordTextVisible == Visibility.Collapsed && !string.IsNullOrEmpty(Model.Password) && !string.IsNullOrEmpty(Model.Key);
                };
                Model.onPropertyChanged();
                value.PrimaryButtonClick += (dialog, _) =>
                {
                    dialog.Hide();
                    onClose.Invoke();
                    operation.FinishSuccess(new Tuple<string, string, string, string>(Model.Host,Model.Login,Model.Password,Model.Key));
                };
            }
        }

        public void Cancel()
        {
            onClose.Invoke();
            operation.FinishFail();
        }
    }

    public class DatabaseCreateDialogModel : PropertyChangable
    {
        public bool HasSecondField;
        public Visibility isDoubleInput => HasSecondField ? Visibility.Visible : Visibility.Collapsed;
        private String password;
        public String Password
        {
            get => password; set
            {
                password = value;
                onPropertyChanged();
                onPropertyChanged(nameof(SamePasswordTextVisible));
            }
        }
        public String passwordRepeat;

        public String PasswordRepeat
        {
            get => passwordRepeat; set
            {
                passwordRepeat = value;
                onPropertyChanged();
                onPropertyChanged(nameof(SamePasswordTextVisible));
            }
        }
        public Visibility SamePasswordTextVisible
        {
            get
            {
                if(!HasSecondField)
                    return Visibility.Collapsed;
                if (password == passwordRepeat)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }
        private String key;
        public String Key
        {
            get => key; set
            {
                key = value;
                onPropertyChanged();
                onPropertyChanged(nameof(SameKeyTextVisible));
            }
        }
        public String keyRepeat;

        public String KeyRepeat
        {
            get => keyRepeat; set
            {
                keyRepeat = value;
                onPropertyChanged();
                onPropertyChanged(nameof(SameKeyTextVisible));
            }
        }
        public Visibility SameKeyTextVisible
        {
            get
            {
                if(!HasSecondField)
                    return Visibility.Collapsed;
                if (key == keyRepeat)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
        }

        private string host;

        public string Host
        {
            get => host;
            set
            {
                this.host = value;
                onPropertyChanged();
            }
        }
        private string login;

        public string Login
        {
            get => login;
            set
            {
                this.login = value;
                onPropertyChanged();
            }
        }
    }
}
