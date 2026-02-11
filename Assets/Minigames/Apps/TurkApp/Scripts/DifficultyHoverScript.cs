using TMPro;
using UnityEngine;

public class DifficultyHoverScript : MonoBehaviour
{
    public TMP_Text targetText;

    public void SetLowerDifficulty()
    {
        SetTargetDifficulty(TurkPuzzleScript.CurrentDifficutly - 1);
    }

    public void SetHigherDifficulty()
    {
        int targetDifficulty = TurkPuzzleScript.CurrentDifficutly + 1;
        if (targetDifficulty >= TurkPuzzleScript.DifficultiesUnlocked) targetDifficulty = -1;
        SetTargetDifficulty(targetDifficulty);
    }

    public void SetTargetDifficulty(int nextDifficulty)
    {
        int currentDifficulty = TurkPuzzleScript.CurrentDifficutly;
        VisionsDifficultySO difficultyData = TurkPuzzleScript.instance.LevelSets[currentDifficulty];
        string DName = difficultyData.PuzzleSetName;
        string DMultiplier = Mathf.Pow(5, currentDifficulty).ToString();
        string DLightnessMultiplier = difficultyData.DarknessModifier.ToString();
        string DFalconCutoffText = System.TimeSpan.FromSeconds(difficultyData.FalconSpeed).ToString("m\\:ss");
        string DMiloRecordText = System.TimeSpan.FromSeconds(difficultyData.MiloRecord).ToString("m\\:ss"); ;

        string NextName = "?";
        string Multiplier = "?";
        string LightnessMultiplier = "?";
        string FalconCutoffText = "?";
        string MiloRecordText = "?";
        if (nextDifficulty >= 0)
        {
            VisionsDifficultySO nextDifficultyData = TurkPuzzleScript.instance.LevelSets[nextDifficulty];
            NextName = nextDifficultyData.PuzzleSetName;
            Multiplier = Mathf.Pow(5, nextDifficulty).ToString() + "x";
            LightnessMultiplier = nextDifficultyData.DarknessModifier.ToString() + "x";
            float FalconCutoff = nextDifficultyData.FalconSpeed;
            FalconCutoffText = System.TimeSpan.FromSeconds(FalconCutoff).ToString("m\\:ss");
            float MiloRecord = nextDifficultyData.MiloRecord;
            MiloRecordText = System.TimeSpan.FromSeconds(MiloRecord).ToString("m\\:ss");
        }
        else
        {
             
        }
        targetText.text =
            $"{DName}  >> {NextName}\r\n" +
            $"{DMultiplier}x  >> {Multiplier}\r\n" +
            $"{DLightnessMultiplier}x >> {LightnessMultiplier}\r\n" +
            $"{DFalconCutoffText} >> {FalconCutoffText}\r\n" +
            $"{DMiloRecordText} >> {MiloRecordText}";
    }
}
