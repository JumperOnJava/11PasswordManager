using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using Password11.ColorLib;
using Password11.Datatypes;
using Password11.GUI;
using Password11.GUI.StorageDialogs.AccountCreateDialog;
using Password11.src.Util;
using Password11.Util;
using Password11.ViewModel;

namespace Password11;

public sealed partial class AccountEdit : Page
{
    private readonly AccountEditModel model;

    public AccountEdit()
    {
        model = new AccountEditModel();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        try
        {
            model.Target = (AccountEditor)e.Parameter;
            var allTags = new ObservableCollection<Tag>(AccountsListPage.Model.RawTags);
            model.SelectedTags =
                new ObservableCollection<Tag>(allTags.Where(tag => model.Target.Tags.Contains(tag.Identifier)));
            model.UnselectedTags =
                new ObservableCollection<Tag>(allTags.Where(tag => !model.Target.Tags.Contains(tag.Identifier)));
            ColorPickerRing.Color = model.Target.Colors.BaseColor.AsWinColor;
            model.SelectedTags.CollectionChanged += (_, _) =>
            {
                model.Target.Tags = new List<Tag>(model.SelectedTags).Select(e => e.Identifier).ToList();
            };
        }
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException("Passed wrong class as page parameter", ex);
        }
    }

    private void Save(object sender, RoutedEventArgs e)
    {
        var dialog = new AskDialogOperation(this, "Save changes?", "Save", "Cancel");
        dialog.OnFinished += ok =>
        {
            if (ok) model.Target.Finish(true);
        };
    }

    private void Cancel(object sender, RoutedEventArgs e)
    {
        var dialog = new AskDialogOperation(this, "Undo changes?", "Confirm", "Cancel");
        dialog.OnFinished += ok =>
        {
            if (ok) model.Target.Finish(false);
        };
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        model.PasswordRevealMode =
            (bool)PasswordCheck.IsChecked ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden;
    }

    private void ColorPickerRing_OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        model.Target.Colors = new ColorsScheme(new AdvColor(sender.Color));
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var r = (Tag)(sender as ButtonBase).CommandParameter;
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

internal class AccountEditModel : PropertyChangable
{
    private PasswordRevealMode passwordRevealMode;
    private AccountEditor target;

    public AccountEditModel()
    {
        SelectedTags = new ObservableCollection<Tag>();
        UnselectedTags = new ObservableCollection<Tag>();
    }

    public ObservableCollection<Tag> SelectedTags { get; set; }
    public ObservableCollection<Tag> UnselectedTags { get; set; }
    public bool EmailCorrect => new Regex(AccountCreateDialog.EmailPattern).IsMatch(target.Email);
    public Visibility EmailWarningVisibility => EmailCorrect ? Visibility.Collapsed : Visibility.Visible;

    public bool ButtonEnabled
    {
        get
        {
            var isTargetAppFilled = !string.IsNullOrEmpty(target.TargetApp);
            var isPasswordFilled = !string.IsNullOrEmpty(target.Password);
            var isUsernameFilled = !string.IsNullOrEmpty(target.Username);
            var isEmailFilled = !string.IsNullOrEmpty(target.Email);
            var isEmailCorrectOrEmpty = EmailCorrect || !isEmailFilled;

            var enabled = isTargetAppFilled &&
                          isPasswordFilled &&
                          (isUsernameFilled || isEmailFilled) && isEmailCorrectOrEmpty;
            return enabled;
        }
    }

    public PasswordRevealMode PasswordRevealMode
    {
        get => passwordRevealMode;
        set
        {
            passwordRevealMode = value;
            onPropertyChanged();
        }
    }

    public AccountEditor Target
    {
        get => target;
        set
        {
            target = value;
            onPropertyChanged();
            target.PropertyChanged += (_, _) =>
            {
                onPropertyChanged(nameof(EmailCorrect));
                onPropertyChanged(nameof(EmailWarningVisibility));
                onPropertyChanged(nameof(ButtonEnabled));
            };
        }
    }
}