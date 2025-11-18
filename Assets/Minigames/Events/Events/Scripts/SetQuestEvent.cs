using UnityEngine;

[CreateAssetMenu(fileName = "SetQuestEvent", menuName = "Events/SetQuestEvent")]
public class SetQuestEvent : EventAbstract
{
    public string NextQuestName;
    public override void OnActivate()
    {
        QuestManager.CompleteQuest(QuestManager.currentQuest);
        if (NextQuestName.Length > 0) QuestManager.ChangeQuest(NextQuestName);
    }
}
