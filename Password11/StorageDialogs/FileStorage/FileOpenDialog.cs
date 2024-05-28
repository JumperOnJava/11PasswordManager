using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Password11.Datatypes;

namespace Password11.StorageDialogs.FileStorage
{
    public sealed class FileOpenDialog : EmptyOperation<StorageManager>
    {
        private readonly Operation<StorageManager> operation;
        public static bool _opening;
        public FileOpenDialog()
        {
            OpenStorageDialog();
            OnFinished += _ =>
            {
                _opening = false;
            };
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
                    FinishSuccess(new JsonFileStorageManager(new FileByteLocation(file.Path)));
                    return;
                }
                else
                {
                    Finish(false);
                    Console.WriteLine("Operation cancelled.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to open file", ex);
            }
            Finish(false);

            _opening = false;
        }

        public JsonFileStorageManager Manager { get; set; }
    }
}