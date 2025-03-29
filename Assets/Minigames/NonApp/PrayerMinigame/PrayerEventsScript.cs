using DS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerEventsScript : MonoBehaviour
{
    public DSDialogue StartEvent;
    public DSDialogue StartEvent2;
    public DSDialogue AriesMeeting;
    public DSDialogue CrowdworkIntro;

    private void OnEnable()
    {
        PrayerScript.PrayerSubmitted += OnPrayerSubmission;
    }
    private void OnDisable()
    {
        PrayerScript.PrayerSubmitted -= OnPrayerSubmission;
    }

    public void OnPrayerSubmission(bool GoodPrayer)
    {
        if(PrayerScript.TotalPrayerCount == 3)
        {
            StartEvent.SubmitDialogue();
        }
        if (PrayerScript.TotalPrayerCount == 10)
        {
            AriesMeeting.SubmitDialogue();
        }
        if (PrayerScript.TotalPrayerCount == 18)
        {
            StartEvent2.SubmitDialogue();
        }
        if (PrayerScript.TotalPrayerCount == 25)
        {
            CrowdworkIntro.SubmitDialogue();
            CrowdworkIntro.OnMessageComplete += UnlockApps;
        }
    }

    public void UnlockApps()
    {
        AppMenuScript.SetAppsRevealed(2);
    }
}
