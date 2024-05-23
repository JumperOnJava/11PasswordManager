using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WinUi3Test.src.Util
{
    public interface Identifiable<T>
    {
        [JsonRequired]
        public UniqueId<T> Identifier { get; }
    }
    
    public struct UniqueId<T>
    {
        [JsonRequired]
        public long id { get; set; }

        public UniqueId(long id)
        {
            this.id = id;
        }
        public UniqueId()
        {
            this.id = Random.Shared.NextInt64();
        }
    }

}
