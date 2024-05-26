using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Password11.Datatypes;
using Password11.src.Ui;
using Password11.src.Util;
using Password11.ViewModel;

namespace Password11;

public sealed partial class AccountCreateDialog : Page
{
    public const string EmailPattern =
        @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";

    private readonly ContentDialog dialog;
    private readonly AccountCreationModel model;

    public AccountCreateDialog(ContentDialog dialog, AccountEditor account, IList<Tag> tags)
    {
        dialog.Content = this;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Add account";
        dialog.PrimaryButtonText = "Save";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        this.dialog = dialog;
        InitializeComponent();
        dialog.ShowAsync().GetResults();
        dialog.IsPrimaryButtonEnabled = false;
        dialog.PrimaryButtonClick += (a, b) =>
        {
            account.Tags = new List<Tag>(model.SelectedTags).Select(e => e.Identifier).ToList();
            account.Finish(true);
            a.Hide();
        };
        dialog.CloseButtonClick += (a, b) =>
        {
            a.Hide();
            account.Finish(false);
        };
        model = new AccountCreationModel(account);
        model.Account.PropertyChanged += (_, _) => UpdateSaveButton();
        model.PropertyChanged += (_, _) => UpdateSaveButton();
        model.UnselectedTags = new ObservableCollection<Tag>(tags);
        model.SelectedTags = new ObservableCollection<Tag>();
        ColorPickerRing.Color = model.Account.BaseColorBindable;
    }

    private void UpdateSaveButton()
    {
        var isTargetAppFilled = !string.IsNullOrEmpty(model.Account.TargetApp);
        var isPasswordFilled = !string.IsNullOrEmpty(model.Account.Password);
        var isUsernameFilled = !string.IsNullOrEmpty(model.Account.Username);
        var isEmailFilled = !string.IsNullOrEmpty(model.Account.Email);
        var isEmailCorrectOrEmpty = model.EmailCorrect || !isEmailFilled;

        var enabled = isTargetAppFilled &&
                      isPasswordFilled &&
                      (isUsernameFilled || isEmailFilled) && isEmailCorrectOrEmpty;

        dialog.IsPrimaryButtonEnabled = enabled;
    }

    private void ColorPickerRing_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if (args.NewColor == args.OldColor)
            return;
        model.Account.Colors = new ColorsScheme(new AdvColor(args.NewColor));
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var r = (Tag)((ButtonBase)sender).CommandParameter;
        try
        {
            var tag = model.SelectedTags.First(e => e == r);
            model.SelectedTags.Remove(tag);
            model.UnselectedTags.Add(tag);
        }
        catch (Exception ex)
        {
            var tag = model.UnselectedTags.First(e => e == r);
            model.UnselectedTags.Remove(tag);
            model.SelectedTags.Add(tag);
        }
    }
}

internal class AccountCreationModel : PropertyChangable
{
    public AccountCreationModel(AccountEditor account)
    {
        account.PropertyChanged += (_, _) =>
        {
            onPropertyChanged(nameof(EmailCorrect));
            onPropertyChanged(nameof(EmailWarningVisibility));
        };
        Account = account;
    }

    public AccountEditor Account { get; }
    public ObservableCollection<Tag> SelectedTags { get; set; }
    public ObservableCollection<Tag> UnselectedTags { get; set; }
    public bool EmailCorrect => new Regex(AccountCreateDialog.EmailPattern).IsMatch(Account.Email);
    public Visibility EmailWarningVisibility => EmailCorrect ? Visibility.Collapsed : Visibility.Visible;
}