using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WinUi3Test.src.Storage
{
    public interface Storage : Clonable<Storage>
    {
        [JsonInclude]
        public List<Tag> Tags { get; }
        [JsonInclude]
        public List<Account> Accounts { get; }
        public StorageSettings StorageSettings { get; }
    }

    public struct StorageSettings
    {
    }
}
