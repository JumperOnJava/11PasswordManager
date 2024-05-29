using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Password11.Datatypes;
using Password11.StorageManagers;
using WinRT.Interop;

namespace Password11.GUI.Dialogs;

public sealed class FileOpenDialog : Operation<StorageManager>
{
    public static bool _opening;

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
                _opening = false;
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