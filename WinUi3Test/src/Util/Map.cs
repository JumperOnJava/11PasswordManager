using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUi3Test.src.Util
{
    static class MapExtension
    {
        public static List<O> Map<I, O>(this IList<I> collection,Func<I,O> function)
        {
            List<O> list = new List<O>(collection.Count);
            for(int i=0;i<collection.Count;i++) {
                list.Add(function(collection[i]));
            }
            return list;
        }
    }
}
