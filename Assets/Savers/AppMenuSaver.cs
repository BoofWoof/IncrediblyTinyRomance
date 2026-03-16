using PixelCrushers;
using System;
using System.Collections.Generic;

public class AppMenuSaver : Saver
{
    [Serializable]
    public class AppMenuSaveData
    {
        public List<string> UnlockedApps = new List<string>();
    }
    public override string RecordData()
    {
        AppMenuSaveData newSaveData = new AppMenuSaveData()
        {
            UnlockedApps = AppMenuScript.UnlockedApps
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        List<string> unlockedApps = SaveSystem.Deserialize<AppMenuSaveData>(s).UnlockedApps;

        foreach(string appName in unlockedApps)
        {
            AppMenuScript.RevealApp(appName);
        }

    }
}
