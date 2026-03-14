using PixelCrushers;
using System;
using System.Collections.Generic;

public class AchievementStorageScript : Saver
{
    public int Priority;
    public List<AchievementAbstractSO> Achievements;

    public bool SubmitAtStart = false;
    public bool Submitted = false;

    public string AnnouncementText;

    [Serializable]
    public class AchievementDiscoveredSaveData
    {
        public bool Saved = false;
    }

    public override void ApplyData(string s)
    {
        if (SaveSystem.Deserialize<AchievementDiscoveredSaveData>(s).Saved) SubmitAchievementsToListWithAnnouncement(false);
    }

    public override string RecordData()
    {
        AchievementDiscoveredSaveData newSaveData = new AchievementDiscoveredSaveData()
        {
            Saved = Submitted
        };
        return SaveSystem.Serialize(newSaveData);
    }

    override public void Start()
    {
        base.Start();
        if (SubmitAtStart)
        {
            SubmitAchievementsToList();
        }
    }

    public void SubmitAchievementsToList()
    {
        SubmitAchievementsToListWithAnnouncement(true);
    }
    public void SubmitAchievementsToListWithAnnouncement(bool announce)
    {
        if(Submitted) return;
        AchievementListScript.AddAchievementsStatic(Achievements, Priority);
        Submitted = true;

        if (!SubmitAtStart && announce) AchievementListScript.instance.ShowNotification();

        if(announce) MakeAnnouncement();
    }

    public void MakeAnnouncement()
    {
        if (AnnouncementText.Length == 0) return;
        AnnouncementScript.StartAnnouncement(AnnouncementText);
    }
}
