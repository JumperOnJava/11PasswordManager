using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using Password11.Datatypes.Serializing;

namespace Password11.Datatypes;

public class AppSettings
{
    public static AppSettings GLOBAL { get; set; } = new("global_settings.json");
    private string path;
    public List<StorageManager> storageHistory { get; set; } 
    
    private AppSettings(string path)
    {
        this.path = path;
        storageHistory = new List<StorageManager>();
    }
    [JsonConstructor]
    private AppSettings()
    {
    }

    public void Load()
    {
        if (File.Exists(path))
        {
            var target = JsonTools.DeserializeSmart<AppSettings>(File.ReadAllText(path));
            this.path = target.path;
            this.storageHistory = target.storageHistory;
        }
        storageHistory = new List<StorageManager>(storageHistory
            .GroupBy(s => s.DisplayInfo.DisplayPath)
            .Select(s => s.First())
            .ToList());
        Save();
    }
    public void Save()
    {
        storageHistory = new List<StorageManager>(storageHistory.Distinct().ToList());
        File.WriteAllText("global_settings.json", JsonTools.SerializeSmart(this));
    }
}