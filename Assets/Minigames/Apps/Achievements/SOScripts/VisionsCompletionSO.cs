using UnityEngine;

[CreateAssetMenu(fileName = "VisionsCompletionSO", menuName = "Achievements/Visions/Completion")]
public class VisionsCompletionSO : AchievementAbstractSO
{
    public int DifficultyCompletion;

    public override bool CheckCompletionCriteria()
    {
        return TurkPuzzleScript.isDifficultyCompleted(DifficultyCompletion);
    }
}
