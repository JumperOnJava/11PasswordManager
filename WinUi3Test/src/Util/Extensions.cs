using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUi3Test.Datatypes;

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

        public static SaveLoader AesEncryptedStorage(this SaveLoader target, string key)
        {
            return new EncryptedSaveLoader(target, key);
        }

        public static byte[] EncodeUtf8(this string s) => Encoding.UTF8.GetBytes(s);
        public static string DecodeUtf8(this byte[] s) => Encoding.UTF8.GetString(s);
        public static void ShowExceptionOnFail(Action action,XamlRoot xamlRoot)
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
                    XamlRoot = xamlRoot
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
                    XamlRoot = xamlRoot
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
