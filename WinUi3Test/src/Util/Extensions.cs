using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WinUi3Test.src.Util
{
    static class Extensions
    {
        public static List<O> Map<I, O>(this IList<I> collection,Func<I,O> function)
        {
            List<O> list = new List<O>(collection.Count);
            for(int i=0;i<collection.Count;i++) {
                list.Add(function(collection[i]));
            }
            return list;
        }

        public static SerializableString Serializable(this string s)
        {
            return new SerializableString(s);
        }
    }

    class SerializableString : object
    {
        public String s;

        public SerializableString(String s)
        {
            this.s = s;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("string",s);
        }
    }
}
