using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WinUi3Test.src.Storage;

namespace WinUi3Test.src.Util
{
    public interface Identifiable
    {
        [JsonInclude]
        public TagRef Identifier { get; }

    }
}
