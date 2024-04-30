using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.Storage
{
    public interface Account : Identifiable, Taggable
    {
        public string TargetApp { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Account Clone();
    }
}
