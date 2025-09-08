using PixelCrushers.DialogueSystem;
using UnityEngine;

public class VisionEvents : MonoBehaviour
{
    private void OnEnable()
    {
        TurkPuzzleScript.OnPuzzleComplete += OnPuzzleCompletion;
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
            if (allCount >= 3)
            {
                QuestManager.IncrementQuest();
            }
        }
    }
}
