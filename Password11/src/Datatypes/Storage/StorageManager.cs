using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace Password11.Datatypes;

public interface StorageManager
{
    LocationDisplayModel DisplayInfo { get; }
    Task<StorageData> GetData();
    Task SetData(StorageData value);
    public bool IsValid();
    Task<bool> SetupManagerInGui(Page parent);
    void ResetOnFail();
}