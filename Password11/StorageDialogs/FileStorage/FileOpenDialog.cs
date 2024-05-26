using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Password11.Datatypes;
using WinRT.Interop;

namespace Password11.StorageDialogs.FileStorage;

public sealed class FileOpenDialog : EmptyOperation<StorageManager>
{
    public static bool _opening;
    private readonly Operation<StorageManager> operation;

    public FileOpenDialog()
    {
        OpenStorageDialog();
        OnFinished += _ => { _opening = false; };
    }

    public JsonFileStorageManager Manager { get; set; }

    private async Task OpenStorageDialog()
    {
        if (_opening) return;
        _opening = true;
        try
        {
            var openFileDialog = new FileOpenPicker();

            var window = App.MainWindow;

            var hWnd = WindowNative.GetWindowHandle(window);

            InitializeWithWindow.Initialize(openFileDialog, hWnd);

            openFileDialog.FileTypeFilter.Add(".pwdb");
            openFileDialog.FileTypeFilter.Add("*");

            var file = await openFileDialog.PickSingleFileAsync();
            if (file != null)
            {
                FinishSuccess(new JsonFileStorageManager(new FileByteLocation(file.Path)));
                return;
            }

            Finish(false);
            Console.WriteLine("Operation cancelled.");
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to open file", ex);
        }

        Finish(false);

        _opening = false;
    }
}