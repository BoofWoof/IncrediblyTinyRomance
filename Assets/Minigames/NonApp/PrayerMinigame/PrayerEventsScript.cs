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
        int allCount = DialogueLua.GetVariable("PrayersSubmitted").asInt;
        DialogueLua.SetVariable("PrayersSubmitted", allCount + 1);

        if(GoodPrayer)
        {
            int correctCount = DialogueLua.GetVariable("SuccessfulPrayersSubmitted").asInt;
            DialogueLua.SetVariable("SuccessfulPrayersSubmitted", correctCount + 1);
        } else
        {
            int failCount = DialogueLua.GetVariable("FailedPrayersSubmitted").asInt;
            DialogueLua.SetVariable("FailedPrayersSubmitted", failCount + 1);
        }

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
        if (
                QuestLog.GetQuestState("Worship") == QuestState.Active
            )
        {
            QuestManager.QuestManagerInstance.QuickUpdate();
            int correctCount = DialogueLua.GetVariable("SuccessfulPrayersSubmitted").asInt;
            Debug.Log(correctCount);
            if (correctCount >= 10)
            {
                QuestManager.CompleteQuest("Worship");
                MessageQueue.addDialogue("SinfulProposal");
            }
        }
        if (
                QuestLog.GetQuestState("More Worship...") == QuestState.Active
            )
        {
            QuestManager.QuestManagerInstance.QuickUpdate();
            int correctCount = DialogueLua.GetVariable("SuccessfulPrayersSubmitted").asInt;
            Debug.Log(correctCount);
            if (correctCount >= 17)
            {
                QuestManager.CompleteQuest("More Worship...");
                MessageQueue.addDialogue("InstallReady");
            }
        }
    }

    public void UnlockApps()
    {
        AppMenuScript.SetAppsRevealed(2);
    }
}
