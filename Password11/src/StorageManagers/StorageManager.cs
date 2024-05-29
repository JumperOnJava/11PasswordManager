using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Password11.Datatypes;

namespace Password11.StorageManagers;

public interface StorageManager
{
    LocationDisplayModel DisplayInfo { get; }
    Task<StorageData> GetData();
    Task SetData(StorageData value);
    public bool IsValid();
    Task<bool> SetupManagerInGui(Page parent);
    void ResetOnFail();
}