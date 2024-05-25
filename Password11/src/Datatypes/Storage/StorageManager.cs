using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace Password11.Datatypes;

public interface StorageManager
{
    Task<StorageData> GetData();
    Task SetData(StorageData value);
    public bool IsValid();
    LocationDisplayModel DisplayInfo { get; }
    Task<bool> SetupManagerInGui(Page parent);
    void Fail();
}