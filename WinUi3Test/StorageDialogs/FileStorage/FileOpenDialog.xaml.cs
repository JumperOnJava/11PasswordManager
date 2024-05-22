using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinUi3Test.Datatypes;
using WinUi3Test.Dialogs;
using WinUi3Test.src.Util;

namespace WinUi3Test.StorageDialogs.FileStorage
{
    public sealed partial class FileOpenDialog : Page, DialogPage
    {
        private readonly Operation<StorageManager> operation;
        public static bool _opening;

        public event Action onClose;

        public FileOpenDialog(Operation<StorageManager> operation)
        {
            this.operation = operation;
            InitializeComponent();
            OpenStorageDialog().Wait();
            onClose?.Invoke();
        }
        
        private async Task OpenStorageDialog()
        {
            if (_opening) return;
            _opening = true;
            try
            {
                var openFileDialog = new FileOpenPicker();

                var window = App.MainWindow;

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

                WinRT.Interop.InitializeWithWindow.Initialize(openFileDialog, hWnd);

                openFileDialog.FileTypeFilter.Add(".pwdb");
                openFileDialog.FileTypeFilter.Add("*");

                var file = await openFileDialog.PickSingleFileAsync();
                if (file != null)
                {
                    operation.FinishSuccess
                    (
                        new ByteStorageManager
                        (
                            new FileByteLocation(file.Path)
                                .AesEncryptedStorage
                                (
                                    await PasswordDialog.AskPassword(XamlRoot,false)
                                )
                        )
                    );
                }
                else
                {
                    Console.WriteLine("Operation cancelled.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to open file", ex);
            }
            operation.Finish(false);
            _opening = false;
        }
    }
}