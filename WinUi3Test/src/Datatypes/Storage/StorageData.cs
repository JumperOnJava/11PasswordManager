using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using WinUi3Test.Datatypes.Serializing;

namespace WinUi3Test.Datatypes
{
    public class StorageData : Clonable<StorageData>
    {
        [JsonRequired]
        public Dictionary<long, Tag> Tags { get; set; }
        [JsonRequired]
        public List<UniqueTagId> TagsOrder { get; set; }
        [JsonRequired]
        public List<Account> Accounts { get;  set; }
        [JsonRequired]
        public StorageSettings StorageSettings { get; set; }
        public StorageData()
        {
            Tags = new();
            TagsOrder = new();
            Accounts = new();
            StorageSettings = new();
        }
        public StorageData Clone()
        {
            var staticStorage = new StorageData();
            staticStorage.StorageSettings = StorageSettings;
            staticStorage.Accounts = new List<Account>(Accounts);
            staticStorage.TagsOrder = new List<UniqueTagId>(TagsOrder);
            staticStorage.Tags = new Dictionary<long, Tag>(Tags);
            return staticStorage;
        }
    }
    
    public class StorageSettings
    {}
}
