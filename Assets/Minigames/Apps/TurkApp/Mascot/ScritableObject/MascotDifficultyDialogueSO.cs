using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VisionCompletionMascotText
{
    public int CompletionCount;
    [TextArea] public string SolutionDialogues;
    [HideInInspector] public bool Triggered;
}
[Serializable]
public class TimePassingDialogues
{
    public float TimePassed;
    public bool AllowRetrigger;
    [TextArea] public List<string> SolutionDialogues;
    [HideInInspector] public int TriggerOccurances;
}

[CreateAssetMenu(fileName = "MascotDifficultyDialogueSO", menuName = "Visions/MascotDifficultyDialogueSO")]
public class MascotDifficultyDialogueSO : ScriptableObject
{
    [Header("Difficulty Increase")]
    [HideInInspector] public bool FirstIncrease = true;
    public int IncreaseOccurrences = 0;
    [TextArea] public string FirstDifficultyIncreaseDialogues;
    [TextArea] public List<string> DifficultyIncreaseDialogues;

    [Header("Difficulty Decrease")]
    [HideInInspector] public bool FirstDecrease = true;
    public int DecreaseOccurrences = 0;
    [TextArea] public string FirstDifficultyDecreaseDialogues;
    [TextArea] public List<string> DifficultyDecreaseDialogues;

    [Header("Click")]
    public int ClickOccurrences = 0;
    [TextArea] public List<string> ClickDialogues;

    [Header("Spam")]
    public int SpamCloseOccurances = 0;
    [TextArea] public List<string> SpamCloseDialogues;

    [Header("Solution")]
    public List<VisionCompletionMascotText> SetSolutionDialogues;
    public Dictionary<int, VisionCompletionMascotText> SolutionDialogues = new Dictionary<int, VisionCompletionMascotText>();

    [Header("Time")]
    public List<TimePassingDialogues> TimeDialogues;

    [Header("Audio")]
    public AudioClip SpeechSound;

    public void ResetData()
    {
        FirstIncrease = true;
        IncreaseOccurrences = 0;

        FirstDecrease = true;
        DecreaseOccurrences = 0;

        ClickOccurrences = 0;
        SpamCloseOccurances = 0;

        foreach (VisionCompletionMascotText visionCompletionMascotText in SetSolutionDialogues)
        {
            SolutionDialogues[visionCompletionMascotText.CompletionCount] = visionCompletionMascotText;
            visionCompletionMascotText.Triggered = false;
        }

        foreach (TimePassingDialogues tpD in TimeDialogues)
        {
            tpD.TriggerOccurances = 0;
        }
    }
}
