using UnityEngine;

[CreateAssetMenu(fileName = "VisionsCompletionSO", menuName = "Achievements/Visions/Completion")]
public class VisionsCompletionSO : AchievementAbstractSO
{
    public int DifficultyCompletion;

    public override bool CheckCompletionCriteria()
    {
        return TurkPuzzleScript.isDifficultyCompleted(DifficultyCompletion);
    }

    public override string ProgressText()
    {
        int TotalPuzzles = TurkPuzzleScript.instance.LevelSets[DifficultyCompletion].Puzzles.Count;
        int Completed = 0;
        if (TurkPuzzleScript.PuzzlesCompleted.ContainsKey(DifficultyCompletion)) Completed = TurkPuzzleScript.PuzzlesCompleted[DifficultyCompletion];
        if (Completed > TotalPuzzles) Completed = TotalPuzzles;
        return $" ({Completed}/{TotalPuzzles})";
    }
}
