    using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinUi3Test.Datatypes;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;
using WinUi3Test.src.ViewModel;
using WinUi3Test.Util;
using WinUi3Test.ViewModel;

namespace WinUi3Test
{
    public sealed partial class PasswordEdit : Page
    {
        private PasswordEditModel model;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                model.Target = e.Parameter as AccountOperation;
                var allTags = AccountsListPage.Model.Tags.Select(t=>t.Target).ToList();
                model.SelectedTags = new ObservableCollection<UniqueTagId>(model.Target.Tags);
                foreach (var tag in model.SelectedTags)
                {
                    for (var index = 0; index < allTags.Count; index++)
                    {
                        var tag2 = allTags[index];
                        if (tag == tag2)
                        {
                            allTags.RemoveAt(index);
                            continue;
                        }
                    }
                }

                model.UnselectedTags = new ObservableCollection<UniqueTagId>(allTags);
                ColorPickerRing.Color = model.Target.Colors.BaseColor.asWinColor;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException("Passed wrong class as page parameter", ex);
            }
        }
        public PasswordEdit()
        {
            model = new PasswordEditModel();
            InitializeComponent();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var dialog = new DialogOperation(XamlRoot,"Save changes?","Save","Cancel");
            dialog.OnFinished += (ok) =>
            {
                if (ok)
                {
                    model.Target.Tags = new List<UniqueTagId>(model.SelectedTags);
                    model.Target.Finish(true);
                }
            };
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            var dialog = new DialogOperation(XamlRoot,"Undo changes?","Confirm","Cancel");
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
            UniqueTagId r = (UniqueTagId)(sender as ButtonBase).CommandParameter;
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
        private AccountOperation target;
        public ObservableCollection<UniqueTagId> SelectedTags { get; set; }
        public ObservableCollection<UniqueTagId> UnselectedTags {  get; set; }

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
        public AccountOperation Target
        {
            get => target; set
            {
                target = value;
                onPropertyChanged("Target");
            }
        }
    }
}
