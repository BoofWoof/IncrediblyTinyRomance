using PixelCrushers;
using System;

public class QuestSaver : Saver
{
    [Serializable]
    public class QuestSaveData
    {
        public string QuestName;
        public int QuestId;
        public bool WaitingForNewQuest;
    }
    public override string RecordData()
    {
        QuestSaveData newSaveData = new QuestSaveData()
        {
            QuestId = QuestManager.currentQuestIndex,
            QuestName = QuestManager.currentQuest,
            WaitingForNewQuest = QuestManager.QuestManagerInstance.WaitForNewQuest
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        QuestSaveData saveData = SaveSystem.Deserialize<QuestSaveData>(s);

        if (saveData.WaitingForNewQuest)
        {
            QuestManager.currentQuest = saveData.QuestName;
            QuestManager.currentQuestIndex = saveData.QuestId;
            QuestManager.SetTextToWaiting();
        } else
        {
            QuestManager.SetQuestByIndex(saveData.QuestId);
        }
    }
}
