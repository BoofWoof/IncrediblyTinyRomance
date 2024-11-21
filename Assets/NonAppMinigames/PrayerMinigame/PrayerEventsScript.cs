using DS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerEventsScript : MonoBehaviour
{
    public DSDialogue StartEvent;
    public DSDialogue StartEvent2;

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
        if(PrayerScript.TotalPrayerCount == 1)
        {
            StartEvent.SubmitDialogue();
        }
        if (PrayerScript.TotalPrayerCount == 10)
        {
            StartEvent2.SubmitDialogue();
        }
    }
}
