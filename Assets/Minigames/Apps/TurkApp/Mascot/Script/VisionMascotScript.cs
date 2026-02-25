using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisionMascotScript : MonoBehaviour
{
    private static VisionMascotScript instance;
    public TMP_Text TextBoxText;
    public GameObject TextBox;

    public List<MascotDifficultyDialogueSO> DifficultyChangeMessages;

    void Awake()
    {
        instance = this;
        MascotClearText();

        foreach (MascotDifficultyDialogueSO mascotDialogue in  DifficultyChangeMessages)
        {
            mascotDialogue.ResetData();
        }
    }

    public static void SayText(string SayText, bool AllowClickoff = true)
    {
        if (SayText.Length == 0) return;

        instance.MascotSayText(SayText, AllowClickoff);
    }

    private void MascotSayText(string SayText, bool AllowClickoff)
    {
        TextBox.gameObject.SetActive(true);
        TextBoxText.text = SayText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox.GetComponent<RectTransform>());
    }

    public static void ClearText()
    {
        instance.MascotClearText();
    }

    public void MascotClearText()
    {
        TextBox.gameObject.SetActive(false);
    }

    public void OnDifficultyIncrease(int currentDifficulty)
    {
        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];
        if (currentDifficultyDialogue.FirstIncrease)
        {
            currentDifficultyDialogue.FirstIncrease = false;
            MascotSayText(currentDifficultyDialogue.FirstDifficultyIncreaseDialogues, true);
            return;
        }
        MascotSayText(currentDifficultyDialogue.DifficultyIncreaseDialogues[currentDifficultyDialogue.IncreaseOccurrences % currentDifficultyDialogue.DifficultyIncreaseDialogues.Count], true);
        currentDifficultyDialogue.IncreaseOccurrences++;
    }

    public void OnDifficultyDecrease(int currentDifficulty)
    {
        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];
        if (currentDifficultyDialogue.FirstDecrease)
        {
            currentDifficultyDialogue.FirstDecrease = false;
            MascotSayText(currentDifficultyDialogue.FirstDifficultyDecreaseDialogues, true);
            return;
        }
        MascotSayText(currentDifficultyDialogue.DifficultyDecreaseDialogues[currentDifficultyDialogue.DecreaseOccurrences % currentDifficultyDialogue.DifficultyDecreaseDialogues.Count], true);
        currentDifficultyDialogue.DecreaseOccurrences++;
    }
}
