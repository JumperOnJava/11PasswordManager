using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;

namespace Password11.src.Util
{
    static class Extensions
    {
        public static StorageManager AesEncryptedManager(this StorageManager manager, string key)
        {
            return new AesStorageManager(manager,key);
        }

        public static byte[] EncodeUtf8(this string s) => Encoding.UTF8.GetBytes(s);
        public static string DecodeUtf8(this byte[] s) => Encoding.UTF8.GetString(s);
        
        public static byte[] DecodeBase64(this string s) => Convert.FromBase64String(s);
        public static string EncodeBase64(this byte[] s) => Convert.ToBase64String(s);
        public static void ShowExceptionOnFail(Page page,Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (DialogException e)
            {
                var dialog = new ContentDialog
                {
                    PrimaryButtonText = "Ok",
                    Title = e.Title,
                    Content = e.Content,
                    XamlRoot = page.XamlRoot
                };
                dialog.ShowAsync();
            }
            catch (Exception e)
            {
                var dialog = new ContentDialog
                {
                    PrimaryButtonText = "Ok",
                    Title = "Error while executing operation",
                    Content = e.Message+"\n"+e.StackTrace,
                    XamlRoot = page.XamlRoot
                };
                dialog.ShowAsync();
            }
        }
    }
}
class DialogException : Exception
{
    public readonly string Title;
    public readonly string Content;

    public DialogException(string title, string content)
    {
        Title = title;
        Content = content;
    }
}
