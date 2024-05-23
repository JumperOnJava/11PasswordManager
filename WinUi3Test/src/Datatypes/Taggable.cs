using System.Collections.Generic;
using WinUi3Test.src.Util;

namespace WinUi3Test.Datatypes;

public interface Taggable   
{
    public List<UniqueId<Tag>> Tags { get; set; }
}