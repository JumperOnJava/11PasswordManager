using Newtonsoft.Json;

namespace WinUi3Test.src.Util
{
    public interface Identifiable
    {
        [JsonRequired]
        public UniqueId Identifier { get; }
    }
    
    public class UniqueId : Identifiable
    {
        [JsonRequired]
        public long id;

        public UniqueId(long id)
        {
            this.id = id;
        }
        [JsonIgnore]
        public UniqueId Identifier => this;
    }

}
