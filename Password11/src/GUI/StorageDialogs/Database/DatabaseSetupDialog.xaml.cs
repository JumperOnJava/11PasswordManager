using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.src.Util;

namespace Password11.GUI.StorageDialogs.Database;

public sealed partial class DatabaseSetupDialog : Page, DialogPage
{
    private readonly DatabaseSetupDialogModel Model = new();
    private readonly Operation<Tuple<string, string, string, string>> operation;

    public DatabaseSetupDialog(Operation<Tuple<string, string, string, string>> operation,
        bool hasSecondField)
    {
        this.operation = operation;
        Model.HasSecondField = hasSecondField;
        InitializeComponent();
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
                value.IsPrimaryButtonEnabled = Model.SamePasswordTextVisible == Visibility.Collapsed &&
                                               !string.IsNullOrEmpty(Model.Password) &&
                                               !string.IsNullOrEmpty(Model.Key);
            };
            Model.onPropertyChanged();
            value.PrimaryButtonClick += (dialog, _) =>
            {
                dialog.Hide();
                onClose.Invoke();
                operation.FinishSuccess(
                    new Tuple<string, string, string, string>(Model.Host, Model.Login, Model.Password, Model.Key));
            };
        }
    }

    public void Cancel()
    {
        onClose.Invoke();
        operation.FinishFail();
    }
}

public class DatabaseSetupDialogModel : PropertyChangable
{
    public bool HasSecondField;

    private string host;
    private string key;
    public string keyRepeat;
    private string login;
    private string password;
    public string passwordRepeat;
    public Visibility isDoubleInput => HasSecondField ? Visibility.Visible : Visibility.Collapsed;

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

    public string Key
    {
        get => key;
        set
        {
            key = value;
            onPropertyChanged();
            onPropertyChanged(nameof(SameKeyTextVisible));
        }
    }

    public string KeyRepeat
    {
        get => keyRepeat;
        set
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
            if (!HasSecondField)
                return Visibility.Collapsed;
            if (key == keyRepeat)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }
    }

    public string Host
    {
        get => host;
        set
        {
            host = value;
            onPropertyChanged();
        }
    }

    public string Login
    {
        get => login;
        set
        {
            login = value;
            onPropertyChanged();
        }
    }
}