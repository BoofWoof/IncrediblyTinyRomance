using PixelCrushers;
using System;
using System.Collections.Generic;

public class PrayerSaver : Saver
{
    [Serializable]
    public class PrayerSaveData
    {
        public List<string> SentPrayerIDs = new List<string>();
        public int GoodPrayers = 0;
        public int BadPrayers = 0;
        public int TotalPrayers = 0;
    }
    public override string RecordData()
    {
        PrayerSaveData newSaveData = new PrayerSaveData()
        {
            SentPrayerIDs = SPrayerSubmissionScript.SentPrayerIDs,
            GoodPrayers = PrayerScript.GoodPrayerCount,
            BadPrayers = PrayerScript.BadPrayerCount,
            TotalPrayers = PrayerScript.TotalPrayerCount
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        PrayerSaveData saveData = SaveSystem.Deserialize<PrayerSaveData>(s);
        if (saveData == null) return;
        SPrayerSubmissionScript.SentPrayerIDs = saveData.SentPrayerIDs;
        PrayerScript.GoodPrayerCount = saveData.GoodPrayers;
        PrayerScript.BadPrayerCount = saveData.BadPrayers;
        PrayerScript.TotalPrayerCount = saveData.TotalPrayers;
    }
}
