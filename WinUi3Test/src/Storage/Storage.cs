using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUi3Test.src.Storage
{
    public interface Storage
    {
        public IList<Tag> Tags { get; }
        public IList<Account> Accounts { get; }
        public Settings Settings { get; }
        public Storage Clone();
    }

    public struct Settings
    {
    }
}
