using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace WinUi3Test.Storage;

public class AppSettings
{
    public List<string> storageHistory { get; set; }
    public AppSettings()
    {
        storageHistory = new List<string>();
        settings = this;
    }

    public static AppSettings settings { get; set; }

    public static void Load()
    {
        if (File.Exists("global_settings.json"))
        {
            settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("global_settings.json"));
        }
        else
        {
            settings = new AppSettings();
            settings.storageHistory = new List<string>(settings.storageHistory.Distinct().ToList());
            Save();
        }
        settings.storageHistory = new List<string>(settings.storageHistory.Distinct().ToList());
    }
    public static void Save()
    {
        settings.storageHistory = new List<string>(settings.storageHistory.Distinct().ToList());
        File.WriteAllText("global_settings.json", JsonSerializer.Serialize(settings));
    }
}