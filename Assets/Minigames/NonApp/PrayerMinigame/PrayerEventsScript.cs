using DS;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerEventsScript : MonoBehaviour
{

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
        if (
                QuestLog.GetQuestState("A Proper Theocrat") == QuestState.Active
            )
        {
            QuestManager.QuestManagerInstance.QuickUpdate();
            int correctCount = DialogueLua.GetVariable("SuccessfulPrayersSubmitted").asInt;
            Debug.Log(correctCount);
            if (correctCount >= 3)
            {
                QuestManager.IncrementQuest();
                MessageQueue.addDialogue("AriesIntroduction2");
            }
        }

        /*
        if(PrayerScript.TotalPrayerCount == 3)
            {
                //StartEvent.SubmitDialogue();
            }
        if (PrayerScript.TotalPrayerCount == 10)
        {
            //AriesMeeting.SubmitDialogue();
        }
        if (PrayerScript.TotalPrayerCount == 18)
        {
            //StartEvent2.SubmitDialogue();
        }
        if (PrayerScript.TotalPrayerCount == 25)
        {
            //CrowdworkIntro.SubmitDialogue();
            //CrowdworkIntro.OnMessageComplete += UnlockApps;
        }
        */
    }

    public void UnlockApps()
    {
        AppMenuScript.SetAppsRevealed(2);
    }
}
