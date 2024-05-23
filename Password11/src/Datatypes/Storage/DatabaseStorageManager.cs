using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;

namespace Password11.src.Datatypes.Storage
{
    internal class DatabaseStorageManager : StorageManager
    {
        public StorageData Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public LocationDisplayModel DisplayInfo => throw new NotImplementedException();
        public async Task<bool> SetupManagerInGui(Page parent)
        {
            throw new NotImplementedException();
        }

        public void Fail()
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

    }
}
