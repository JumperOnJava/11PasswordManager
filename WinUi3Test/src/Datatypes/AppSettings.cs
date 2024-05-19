using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WinUi3Test.Datatypes.Serializing;

namespace WinUi3Test.Datatypes;

public class AppSettings
{
    public List<SaveLoader> storageHistory { get; set; }
    public AppSettings()
    {
        storageHistory = new List<SaveLoader>();
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
        settings.storageHistory = new List<SaveLoader>(settings.storageHistory
            .GroupBy(s => s.DisplayPath)
            .Select(s => s.First())
            .ToList());
        Save();
    }
    public static void Save()
    {
        settings.storageHistory = new List<SaveLoader>(settings.storageHistory.Distinct().ToList());
        File.WriteAllText("global_settings.json", JsonTools.Serialize(settings));
    }
}