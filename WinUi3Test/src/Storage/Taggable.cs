using System.Collections;
using System.Collections.Generic;

namespace WinUi3Test.src.Storage
{
    public interface Taggable
    {
        public IList<Tag> Tags { get; }
    }
}