using Microsoft.UI.Xaml;
using Password11.Datatypes;
using Password11.src.Util;
using Password11Lib.Util;

namespace Password11.ViewModel;

public class UiAccount : PropertyChangable, Identifiable<Account>
{
    private bool copyMenuVisible;
    private Account target;

    public UiAccount(Account target)
    {
        Target = target;
    }

    public Account Target
    {
        get => target;
        set
        {
            target = value;
            onPropertyChanged();
        }
    }

    public bool CopyMenuVisible
    {
        get => copyMenuVisible;
        set
        {
            copyMenuVisible = value;
            onPropertyChanged();
        }
    }

    public bool EmailVisible => target.Email.Replace(" ", "").Length > 0;
    public Visibility EmailVisibility => EmailVisible ? Visibility.Visible : Visibility.Collapsed;
    public bool UsernameVisible => target.Username.Replace(" ", "").Length > 0;
    public Visibility UsernameVisibility => UsernameVisible ? Visibility.Visible : Visibility.Collapsed;
    public bool AppLinkButtonVisible => target.AppLink.Replace(" ", "").Length > 0;
    public Visibility AppLinkButtonVisibility => AppLinkButtonVisible ? Visibility.Visible : Visibility.Collapsed;
    public UniqueId<Account> Identifier => target.Identifier;
}