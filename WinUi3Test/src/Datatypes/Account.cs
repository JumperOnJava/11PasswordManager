using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.UI;
using WinUi3Test.src.Ui;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.Storage
{
    public interface Account : Identifiable, Taggable, INotifyPropertyChanged, Clonable<Account>
    {
        public string TargetApp { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public Type AccountEditor => typeof(PasswordEdit);
        public ColorsScheme Colors { get; set; }
        public Color BaseColorBindable { get; set; }
    }
}
