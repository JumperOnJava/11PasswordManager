    using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls.Primitives;
using Password11.Datatypes;
using Password11.src.Ui;
using Password11.src.Util;
using Password11.Util;
using Password11.ViewModel;
using Password11.src.ViewModel;

namespace Password11
{
    public sealed partial class AccountEdit : Page
    {
        private PasswordEditModel model;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                model.Target = (AccountEditor)e.Parameter;
                var allTags = new ObservableCollection<Tag>(AccountsListPage.Model.RawTags);
                model.SelectedTags = new ObservableCollection<Tag>(allTags.Where(e => model.Target.Tags.Contains(e.Identifier)));
                model.UnselectedTags = new ObservableCollection<Tag>(allTags.Where(e => !model.Target.Tags.Contains(e.Identifier)));   
                ColorPickerRing.Color = model.Target.Colors.BaseColor.asWinColor;
                model.SelectedTags.CollectionChanged += (_, _) =>
                {
                    model.Target.Tags = new List<Tag>(model.SelectedTags).Select(e=>e.Identifier).ToList();
                };
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException("Passed wrong class as page parameter", ex);
            }
        }
        public AccountEdit()
        {
            model = new PasswordEditModel();
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var dialog = new AskDialogOperation(this,"Save changes?","Save","Cancel");
            dialog.OnFinished += (ok) =>
            {
                if (ok)
                {
                    model.Target.Finish(true);
                }
            };
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            var dialog = new AskDialogOperation(this,"Undo changes?","Confirm","Cancel");
            dialog.OnFinished += (ok) =>
            {
                if (ok)
                {
                    model.Target.Finish(false);
                }
            };
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            model.PasswordRevealMode = ((bool)PasswordCheck.IsChecked) ? PasswordRevealMode.Visible : PasswordRevealMode.Hidden; 
        }

        private void ColorPickerRing_OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            model.Target.Colors = new ColorsScheme(new AdvColor(sender.Color));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Tag r = (Tag)(sender as ButtonBase).CommandParameter;
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

    internal class PasswordEditModel : PropertyChangable
    {
        private AccountEditor target;
        public ObservableCollection<Tag> SelectedTags { get; set; }
        public ObservableCollection<Tag> UnselectedTags {  get; set; }

        private PasswordRevealMode passwordRevealMode;
        public PasswordEditModel()
        {
            SelectedTags = new();
            UnselectedTags = new();
        }
        public PasswordRevealMode PasswordRevealMode
        {
            get => passwordRevealMode; set
            {
                passwordRevealMode = value;
                onPropertyChanged("PasswordRevealMode");
            }
        }
        public AccountEditor Target
        {
            get => target; set
            {
                target = value;
                onPropertyChanged("Target");
            }
        }
    }
}
