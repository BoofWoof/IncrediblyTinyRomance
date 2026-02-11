using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisionsSpeedSO", menuName = "Achievements/Visions/Speed")]
public class VisionsSpeedSO : AchievementAbstractSO
{
    public List<int> SpeedCompletions;

    public override bool CheckCompletionCriteria()
    {
        foreach (int i in SpeedCompletions)
        {
            if (!CheckSpeedCompletion(i)) return false;
        }

        return true;
    }

    public bool CheckSpeedCompletion(int difficulty)
    {
        if(!TurkPuzzleScript.TimeRecords.ContainsKey(difficulty)) return false;
        return TurkPuzzleScript.TimeRecords[difficulty] < TurkPuzzleScript.instance.LevelSets[TurkPuzzleScript.CurrentDifficutly].MiloRecord;
    }

    public override string ProgressText()
    {
        int TotalPuzzles = 0;
        foreach (int i in SpeedCompletions)
        {
            if (!CheckSpeedCompletion(i)) break;
            TotalPuzzles++;
        }
        return $" ({TotalPuzzles}/{SpeedCompletions.Count})";
    }
}
