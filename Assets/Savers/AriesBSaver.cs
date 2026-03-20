using PixelCrushers;
using System;

public class AriesBSaver : Saver
{
    [Serializable]
    public class AriesBehaviorSaveData
    {
        public float Anger = 0f;
    }

    public override string RecordData()
    {
        AriesBehaviorSaveData newSaveData = new AriesBehaviorSaveData()
        {
            Anger = PrayerScript.instance.RamAngyLevel
        };
        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        AriesBehaviorSaveData saveData = SaveSystem.Deserialize<AriesBehaviorSaveData>(s);

        if (saveData == null) return;

        PrayerScript.instance.ResumeAngerLevel = saveData.Anger;
        OverworldBehavior.AriesBehavior("instant_judge");

        AriesMoodScript.instance.InstantUpdateMood(saveData.Anger);
    }
}
