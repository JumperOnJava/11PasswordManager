using System.Collections.Generic;
using WinUi3Test.src.Storage;

namespace WinUi3Test.Datatypes
{
    public interface Storage : Clonable<Storage>
    {
        Dictionary<long, Tag> Tags { get; }
        List<TagRef> TagsOrder { get; }
        List<Account> Accounts { get; }
        public StorageSettings StorageSettings { get; }
        public void Save();
        public void Load();
    }

    public struct StorageSettings
    {
    }
}
