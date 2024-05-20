using System.Collections.Generic;

namespace WinUi3Test.Datatypes;

public interface Taggable
{
    public List<UniqueTagId> Tags { get; set; }
}