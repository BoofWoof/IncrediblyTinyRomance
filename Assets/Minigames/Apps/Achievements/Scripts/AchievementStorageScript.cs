using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AchievementStorageScript : MonoBehaviour
{
    public int Priority;
    public List<AchievementAbstractSO> Achievements;

    public bool SubmitAtStart = false;
    public bool Submitted = false;

    public string AnnouncementText;

    public void Start()
    {
        if (SubmitAtStart)
        {
            SubmitAchievementsToList();
        }
    }

    public void SubmitAchievementsToList()
    {
        if(Submitted) return;
        AchievementListScript.AddAchievementsStatic(Achievements, Priority);
        Submitted = true;

        MakeAnnouncement();
    }

    public void MakeAnnouncement()
    {
        if (AnnouncementText.Length == 0) return;
        AnnouncementScript.StartAnnouncement(AnnouncementText);
    }
}
