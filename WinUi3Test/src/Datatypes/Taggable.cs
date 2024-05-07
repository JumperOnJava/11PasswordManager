using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WinUi3Test.src.Storage
{
    public interface Taggable
    {
        public List<TagRef> Tags { get; set; }
    }
}