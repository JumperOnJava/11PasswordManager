using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Password11.Datatypes.Serializing;
using Password11.Dialogs;

namespace Password11.Datatypes;

public class AppSettings
{
    private string path;

    private AppSettings(string path)
    {
        this.path = path;
        storageHistory = new List<StorageManager.StorageManager>();
    }

    [JsonConstructor]
    private AppSettings()
    {
    }

    public static AppSettings GLOBAL { get; set; } = new("global_settings.json");
    public List<StorageManager.StorageManager> storageHistory { get; set; }

    public void Load(StartScreen page)
    {
        if (File.Exists(path))
        {
            AppSettings target;
            try
            {
                 target = JsonTools.DeserializeSmart<AppSettings>(File.ReadAllText(path));
            }
            catch(Exception e)
            {
                ExceptionDialog.ShowException(page,e);
                return;
            }
            storageHistory = target.storageHistory;
        }

        storageHistory = new List<StorageManager.StorageManager>(storageHistory
            .GroupBy(s => s.DisplayInfo.DisplayPath)
            .Select(s => s.First())
            .ToList());
        Save();
    }

    public void Save()
    {
        storageHistory = new List<StorageManager.StorageManager>(storageHistory.Distinct().ToList());
        File.WriteAllText("global_settings.json", JsonTools.SerializeSmart(this));
    }
}