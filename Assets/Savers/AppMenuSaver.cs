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
        AppMenuSaveData saveData = SaveSystem.Deserialize<AppMenuSaveData>(s);

        if (saveData == null) return;

        List<string> unlockedApps = saveData.UnlockedApps;

        foreach(string appName in unlockedApps)
        {
            AppMenuScript.RevealApp(appName);
        }

    }
}
