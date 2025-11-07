using PixelCrushers.DialogueSystem;
using UnityEngine;

public class VisionEvents : MonoBehaviour
{
    private void OnEnable()
    {
        TurkPuzzleScript.OnPuzzleComplete += OnPuzzleCompletion;
        UpgradeScreenScript.UpgradeBoughtEvent += OnUpgradeBought;
    }
    private void OnDisable()
    {
        TurkPuzzleScript.OnPuzzleComplete -= OnPuzzleCompletion;
    }

    public void OnPuzzleCompletion(int PuzzlesComplete, TurkPuzzleScript puzzleScript)
    {
        int allCount = DialogueLua.GetVariable("PuzzlesCompleted").asInt + 1;
        DialogueLua.SetVariable("PuzzlesCompleted", allCount);

        if (
                QuestLog.GetQuestState("Visions") == QuestState.Active
            )
        {
            QuestManager.QuestManagerInstance.QuickUpdate();
            Debug.Log(allCount);
            if (allCount >= 4)
            {
                MessageQueue.addDialogue("FirstPuzzlesVision");
                QuestManager.CompleteQuest("Visions");
            }
        }
    }

    public void OnUpgradeBought(Minigame minigame)
    {
        if (minigame != Minigame.Visions) return;
        int allCount = DialogueLua.GetVariable("PuzzleUpgradesBought").asInt + 1;
        DialogueLua.SetVariable("PuzzleUpgradesBought", allCount);
    }
}
