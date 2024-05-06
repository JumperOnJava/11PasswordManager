using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.Storage
{
    public class StaticStorage : Storage
    {
        [JsonInclude]
        public List<Account> Accounts = new List<Account>()
            {};
        private static Tag GAMING;
        private static Tag SOCIAL;
        static StaticStorage(){
            GAMING = new TagBasic("Gaming");
            SOCIAL = new TagBasic("Social");
            instance = new StaticStorage();
        }
        public static StaticStorage instance;
        [JsonInclude]
        public List<Tag> Tags = new List<Tag>();
        List <Tag> Storage.Tags => Tags;   

        List<Account> Storage.Accounts => Accounts;
        public StorageSettings StorageSettings => storageSettings;
        private StorageSettings storageSettings;
        public Storage Clone()
        {
            var storage = new StaticStorage();
            storage.Accounts = Accounts.Map(e => e.Clone());
            storage.Tags = Tags.Map(e => e);
            storage.storageSettings = new StorageSettings();
            return storage;
        }

    }
}
