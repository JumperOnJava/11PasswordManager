using System.Collections.Generic;
using Password11.src.Util;
using Password11Lib.Util;

namespace Password11.Datatypes;

public interface Taggable   
{
    public List<UniqueId<Tag>> Tags { get; set; }
}