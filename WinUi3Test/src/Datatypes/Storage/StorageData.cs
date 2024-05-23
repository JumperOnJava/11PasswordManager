using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using WinUi3Test.Datatypes.Serializing;
using WinUi3Test.src.ViewModel;

namespace WinUi3Test.Datatypes
{
    public class StorageData : RefClonable<StorageData>
    {
        [JsonRequired] public Dictionary<long, Tag> Tags { get; set; } = new();
        [JsonRequired] public Dictionary<long, Account> Accounts { get; set; } = new();
        [JsonRequired] public List<long> TagsOrder { get; set; } = new();
        [JsonRequired] public List<long> AccountsOrder { get; set; } = new();

        public StorageData CloneRef()
        {
            var staticStorage = new StorageData();
            staticStorage.Tags = new Dictionary<long, Tag>(Tags)
                .Select(kvp=>new KeyValuePair<long,Tag>(kvp.Key,kvp.Value.CloneRef()))
                .ToDictionary(k=>k.Key,k=>k.Value);
            staticStorage.Accounts = new Dictionary<long, Account>(Accounts)
                .Select(kvp=>new KeyValuePair<long,Account>(kvp.Key,kvp.Value.CloneRef()))
                .ToDictionary(k=>k.Key,k=>k.Value);;
            staticStorage.TagsOrder = new List<long>(TagsOrder);
            staticStorage.AccountsOrder = new List<long>(AccountsOrder);
            return staticStorage;
        }

        public void Restore(StorageData state)
        {
            this.Tags = state.Tags;
            this.Accounts = state.Accounts;
            this.TagsOrder = state.TagsOrder;
            this.AccountsOrder = state.AccountsOrder;
        }

        public List<Account> AccountsList()
        {
            return AccountsOrder.Select(id=>
            {
                return Accounts[id];
            }).ToList();
        }
        public List<Tag> TagsList()
        {
            return TagsOrder.Select(id=>
            {
                return Tags[id];
            }).ToList();
        }
    }
}
