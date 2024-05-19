using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WinUi3Test.Datatypes;

namespace WinUi3Test.src.Util
{
    public interface Identifiable
    {
        [JsonRequired]
        public TagRef Identifier { get; }

    }
}
