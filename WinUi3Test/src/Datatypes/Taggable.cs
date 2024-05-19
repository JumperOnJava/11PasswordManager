using System.Collections.Generic;

namespace WinUi3Test.Datatypes;

public interface Taggable
{
    public List<TagRef> Tags { get; set; }
}