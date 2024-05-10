using System.Collections.Generic;
using System.Text.Json.Serialization;
using WinUi3Test.src.Storage;

namespace WinUi3Test.Datatypes
{
    public class FileStorage : Storage
    {
        [JsonInclude]
        public Dictionary<long, Tag> Tags { get; set; }
        [JsonInclude]
        public List<TagRef> TagsOrder { get; set; }
        [JsonInclude]
        public List<Account> Accounts { get;  set; }
        [JsonInclude]
        public StorageSettings StorageSettings { get; set; }

        public FileStorage()
        {
            Tags = new();
            TagsOrder = new();
            Accounts = new();
            StorageSettings = new();
        }
        public Storage Clone()
        {
            var staticStorage = new FileStorage();
            staticStorage.StorageSettings = StorageSettings;
            staticStorage.Accounts = new List<Account>(Accounts);
            staticStorage.TagsOrder = new List<TagRef>(TagsOrder);
            staticStorage.Tags = new Dictionary<long, Tag>(Tags);
            return staticStorage;
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }

        public void Load()
        {
            throw new System.NotImplementedException();
        }
    }
}
