using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using WinUi3Test.Datatypes;
using WinUi3Test.Dialogs;
using WinUi3Test.src.Util;

namespace WinUi3Test.StorageDialogs.FileStorage
{
    public sealed partial class FileCreateDialog : Page, DialogPage
    {
        private Operation<StorageManager> operation;

        public FileCreateDialog(Operation<StorageManager> operation)
        {
            this.operation = operation;
            this.InitializeComponent();
            CreateNew().Wait();
            onClose?.Invoke();
        }

        public event Action onClose;

        private async Task CreateNew()
        {
            if (FileOpenDialog._opening) return;
            FileOpenDialog._opening = true;
            try
            {
                var saveFIleDialog = new FileSavePicker();
                var window = App.MainWindow;

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                WinRT.Interop.InitializeWithWindow.Initialize(saveFIleDialog, hWnd);

                saveFIleDialog.FileTypeChoices.Add("Password storage", new List<string> { ".pwdb" });

                var file = await saveFIleDialog.PickSaveFileAsync();
                if (file != null)
                {
                    var pw = await PasswordDialog.AskPassword(XamlRoot,true);
                    Extensions.ShowExceptionOnFail( (() =>
                    {
                        var saveLoader = new FileByteLocation(file.Path).AesEncryptedStorage(pw);
                        operation.FinishSuccess(new ByteStorageManager(saveLoader,new StorageData()));
                    }), XamlRoot);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create file",ex);
            }
            operation.Finish(false);
            FileOpenDialog._opening = false;
        }

    }
}
