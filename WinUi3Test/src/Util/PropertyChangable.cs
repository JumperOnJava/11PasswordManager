using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WinUi3Test.src.Util
{
    public class PropertyChangable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void onPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}