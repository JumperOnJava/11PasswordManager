using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Microsoft.UI.Xaml;
using Password11.Datatypes;
using Password11.src.Util;
using WinRT.Interop;

namespace Password11.StorageDialogs.FileStorage;

public sealed class FileCreateDialog : EmptyOperation<StorageManager>
{
    public FileCreateDialog()
    {
        OnFinished += _ => { FileOpenDialog._opening = false; };
        CreateNew();
    }

    private async Task CreateNew()
    {
        if (FileOpenDialog._opening) return;
        FileOpenDialog._opening = true;
        try
        {
            var saveFIleDialog = new FileSavePicker();
            var window = App.MainWindow;

            var hWnd = WindowNative.GetWindowHandle(window);

            InitializeWithWindow.Initialize(saveFIleDialog, hWnd);

            saveFIleDialog.FileTypeChoices.Add("Password storage", new List<string> { ".pwdb" });

            var file = await saveFIleDialog.PickSaveFileAsync();
            if (file != null)
                FinishSuccess(JsonFileStorageManager.CreateNew(new FileByteLocation(file.Path)));
            else
                Finish(false);
        }
        catch (Exception ex)
        {
            Finish(false);
            throw new Exception("Failed to create file", ex);
        }

        FileOpenDialog._opening = false;
        Finish(false);
    }
}

public class FileCreateDialogModel : PropertyChangable
{
    private string password1;
    private string password2;
    public FileByteLocation RawByteLocation { get; set; }

    public string Password1
    {
        get => password1;
        set
        {
            password1 = value;
            onPropertyChanged();
            onPropertyChanged("PasswordsCorrect");
            onPropertyChanged("MatchAndExistTextVisible");
        }
    }

    public string Password2
    {
        get => password2;
        set
        {
            password2 = value;
            onPropertyChanged();
            onPropertyChanged("PasswordsCorrect");
            onPropertyChanged("MatchAndExistTextVisible");
        }
    }

    public bool PasswordsCorrect => Password1 == Password2 && !string.IsNullOrEmpty(Password1);
    public Visibility MatchAndExistTextVisible => PasswordsCorrect ? Visibility.Collapsed : Visibility.Visible;
}