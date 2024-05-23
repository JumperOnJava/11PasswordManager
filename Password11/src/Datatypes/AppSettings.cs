using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Password11.Datatypes.Serializing;

namespace Password11.Datatypes;

public class AppSettings
{
    public List<StorageManager> storageHistory { get; set; }
    public AppSettings()
    {
        storageHistory = new List<StorageManager>();
        settings = this;
    }

    public static AppSettings settings { get; set; }

    public static void Load()
    {
        if (File.Exists("global_settings.json"))
        {
            settings = JsonTools.Deserialize<AppSettings>(File.ReadAllText("global_settings.json"));
        }
        else
        {
            settings = new AppSettings();
        }
        settings.storageHistory = new List<StorageManager>(settings.storageHistory
            .GroupBy(s => s.DisplayInfo.DisplayPath)
            .Select(s => s.First())
            .ToList());
        Save();
    }
    public static void Save()
    {
        settings.storageHistory = new List<StorageManager>(settings.storageHistory.Distinct().ToList());
        File.WriteAllText("global_settings.json", JsonTools.Serialize(settings));
    }
}