using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WinUi3Test.src.Util
{
    public interface Identifiable
    {
        [JsonInclude]
        public long Identifier { get; }

    }
}
