using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MascotDifficultyDialogueSO", menuName = "Visions/MascotDifficultyDialogueSO")]
public class MascotDifficultyDialogueSO : ScriptableObject
{
    [HideInInspector] public bool FirstIncrease = true;
    public int IncreaseOccurrences = 0;
    [TextArea] public string FirstDifficultyIncreaseDialogues;
    [TextArea] public List<string> DifficultyIncreaseDialogues;

    [HideInInspector] public bool FirstDecrease = true;
    public int DecreaseOccurrences = 0;
    [TextArea] public string FirstDifficultyDecreaseDialogues;
    [TextArea] public List<string> DifficultyDecreaseDialogues;

    public int ClickOccurrences = 0;
    [TextArea] public List<string> ClickDialogues;

    public void ResetData()
    {
        FirstIncrease = true;
        IncreaseOccurrences = 0;

        FirstDecrease = true;
        DecreaseOccurrences = 0;

        ClickOccurrences = 0;
    }
}
