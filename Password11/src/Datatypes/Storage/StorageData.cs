using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Password11.Datatypes.Serializing;
using Password11.src.Util;
using Password11.src.ViewModel;

namespace Password11.Datatypes
{
    public class StorageData : RefClonable<StorageData>
    {
        [JsonRequired] public List<Tag> Tags { get; set; } = new();
        [JsonRequired] public List<Account> Accounts { get; set; } = new();

        public StorageData CloneRef()
        {
            var staticStorage = new StorageData();
            staticStorage.Tags = new List<Tag>(Tags.Select(t=>t.CloneRef()));
            staticStorage.Accounts = new List<Account>(Accounts.Select(a=>a.CloneRef()));
            return staticStorage;
        }
        public void Restore(StorageData state)
        {
            Accounts = state.Accounts;
            Tags = state.Tags;
        }
    }
}
