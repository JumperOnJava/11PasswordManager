using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Password11.src.Util
{
    public static class Extensions
    {
        public static byte[] EncodeUtf8(this string s) => Encoding.UTF8.GetBytes(s);
        public static string DecodeUtf8(this byte[] s) => Encoding.UTF8.GetString(s);
        
        public static byte[] DecodeBase64(this string s) => Convert.FromBase64String(s);
        public static string EncodeBase64(this byte[] s) => Convert.ToBase64String(s);

        public static Uri Endpoint(this Uri uri, string path)
        {
            return new Uri(uri, path);
        }
    }
}
