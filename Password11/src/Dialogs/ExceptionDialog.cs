using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;
using Password11.src.Util;

namespace Password11.Dialogs;

public static class ExceptionDialog
{
    public static async Task ShowExceptionOnFail(Page page,Action action)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception e)
        {
            await ShowException(page, e);
        }
        
    }

    public static async Task ShowException(Page page, Exception e)
    {
        AddExceptions(e);
        if(alreadyhandled)
            return;
        alreadyhandled = true;
        while (Exceptions.Any())
        {
            var exception = Exceptions.Dequeue();
            if (exception is AggregateException)
            {
                AddExceptions(exception);
            }
            else
            if (exception is DialogException dialogException)
            {
                var dialog = new ContentDialog
                {
                    PrimaryButtonText = "Ok",
                    Title = dialogException.Title,
                    Content = dialogException.Content,
                    XamlRoot = page.XamlRoot
                };
                await dialog.ShowAsync();
            }
            else
            if (exception is Exception)
            {
                var dialog = new ContentDialog
                {
                    PrimaryButtonText = "Ok",
                    Title = "Error while executing operation",
                    Content = exception.Message,
                    XamlRoot = page.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
        alreadyhandled = false;
    }

    private static Queue<Exception> Exceptions = new();
    private static bool alreadyhandled;

    private static void AddExceptions(Exception exception)
    {
        if (exception is DialogException dialogExceptiongException)
        {
            Exceptions.Enqueue(dialogExceptiongException);
        }
        else
        if (exception is AggregateException aggregateException)
        {

            bool EnqueueHandle(Exception exc)
            {
                AddExceptions(exc);
                return true;
            }
            aggregateException.Handle(EnqueueHandle);
        }
        else
        {
            Exceptions.Enqueue(exception);
        }
    }

    public class DialogException : Exception
    {
        public readonly string Title;
        public readonly string Content;

        public DialogException(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}