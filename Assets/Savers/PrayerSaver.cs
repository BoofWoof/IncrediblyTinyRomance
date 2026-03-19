using PixelCrushers;
using System;
using System.Collections.Generic;

public class PrayerSaver : Saver
{
    [Serializable]
    public class PrayerSaveData
    {
        public List<string> SentPrayerIDs = new List<string>();
    }
    public override string RecordData()
    {
        PrayerSaveData newSaveData = new PrayerSaveData()
        {
            SentPrayerIDs = SPrayerSubmissionScript.SentPrayerIDs
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        PrayerSaveData saveData = SaveSystem.Deserialize<PrayerSaveData>(s);
        if (saveData == null) return;
        SPrayerSubmissionScript.SentPrayerIDs = saveData.SentPrayerIDs;
    }
}
