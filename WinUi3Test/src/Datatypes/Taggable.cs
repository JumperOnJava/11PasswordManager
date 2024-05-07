using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WinUi3Test.src.Storage
{
    public interface Taggable
    {
        public ObservableCollection<Tag> Tags { get; }
    }
}