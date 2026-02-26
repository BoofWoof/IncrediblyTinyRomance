using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class VisionMascotScript : MonoBehaviour
{
    private static VisionMascotScript instance;
    public static bool EnableProgress = true;

    public TMP_Text NameText;

    public TMP_Text TextBoxText;
    public GameObject TextBox;

    public GameObject ButtonSetPrefab;
    public GameObject TextFieldPrefab;

    public List<MascotDifficultyDialogueSO> DifficultyChangeMessages;

    public GameObject FocusPanel;

    public AudioSource SpeechAudioSource;

    private bool TextChainActive = false;
    private bool WaitForText = false;
    public bool WaitForInteraction = false;
    public bool SkipWait = false;

    public bool DialogueActive = false;

    private List<Coroutine> WaitCoroutines = new List<Coroutine>();

    void Awake()
    {
        instance = this;
        MascotClearText();

        NameText.text = "";

        FocusPanel.SetActive(false);

        foreach (MascotDifficultyDialogueSO mascotDialogue in  DifficultyChangeMessages)
        {
            mascotDialogue.ResetData();
        }
    }

    public static void SayText(string text)
    {
        if (text.Length == 0) return;

        instance.MascotSayText(text);
    }

    private void MascotSayText(string SayText)
    {
        TextBox.gameObject.SetActive(true);

        string[] SplitText = SayText.Split("<n>");
        if(SplitText.Length > 0)
        {
            StartCoroutine(MascotSayTextChain(SplitText));
            return;
        }

    }
    private void ShowText(string SayText)
    {
        SpeechAudioSource.Play();

        ChoiceBlock choices = SayText.SplitByChoices();
        if (choices.ChoiceDictionary.Count > 0)
        {
            FocusPanel.SetActive(true);

            WaitForInteraction = true;

            TextBoxText.text = choices.PreText;

            GameObject textboxButtons = Instantiate(ButtonSetPrefab, TextBox.transform);
            textboxButtons.transform.localScale = Vector3.one;
            textboxButtons.transform.localRotation = Quaternion.identity;
            textboxButtons.transform.localPosition = Vector3.zero;

            textboxButtons.GetComponent<MascotButtonScript>().SetChoices(choices);

            LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox.GetComponent<RectTransform>());
            return;
        }

        if (SayText.Contains("<name>"))
        {
            SayText = SayText.Replace("<name>", "").Trim();
            WaitForInteraction = true;

            TextBoxText.text = SayText;

            GameObject textField = Instantiate(TextFieldPrefab, TextBox.transform);
            textField.transform.localScale = Vector3.one;
            textField.transform.localRotation = Quaternion.identity;
            textField.transform.localPosition = Vector3.zero;

            MascotTextFieldScript mascotTextFieldScript = textField.GetComponent<MascotTextFieldScript>();
            mascotTextFieldScript.SetTextLimit(2, 6);
            mascotTextFieldScript.OnTextSubmission.AddListener(OnNameSet);

            LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox.GetComponent<RectTransform>());
            return;
        }

        TextBoxText.text = SayText;
        LayoutRebuilder.ForceRebuildLayoutImmediate(TextBox.GetComponent<RectTransform>());

    }

    public IEnumerator MascotSayTextChain(string[] TextChain)
    {
        DialogueActive = true;

        if (TextChain.Length > 1) FocusPanel.SetActive(true);
        TextChainActive = true;
        foreach (string s in TextChain)
        {
            ShowText(s);
            while (WaitForInteraction) { yield return null; }
            if (!SkipWait) yield return new WaitForSeconds(1f);
            WaitForText = true;
            if (!SkipWait) while (WaitForText) { yield return null; }
            SkipWait = false;
        }
        TextChainActive = false;
        MascotClearText();

        DialogueActive = false;
    }

    public static void ClearText()
    {
        instance.MascotClearText();
    }

    public void MascotClearText()
    {
        TextBox.gameObject.SetActive(false);
        FocusPanel.SetActive(false);
    }

    public IEnumerator TimerDialogue(TimePassingDialogues timerDialogueData)
    {
        if (!timerDialogueData.AllowRetrigger && timerDialogueData.TriggerOccurances > 0) yield break;

        float timePassed = 0f;

        while (timePassed < timerDialogueData.TimePassed)
        {
            if (!DialogueActive) timePassed += Time.deltaTime;
            yield return null;
        }

        string dialogue = timerDialogueData.SolutionDialogues[timerDialogueData.TriggerOccurances%timerDialogueData.SolutionDialogues.Count];

        MascotSayText(dialogue);

        timerDialogueData.TriggerOccurances++;
    }

    public void OnPuzzleGeneration()
    {
        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[TurkPuzzleScript.CurrentDifficutly];

        WaitCoroutines = new List<Coroutine>();
        foreach (TimePassingDialogues tpData in currentDifficultyDialogue.TimeDialogues)
        {
            WaitCoroutines.Add(StartCoroutine(TimerDialogue(tpData)));
        }

        if (!TurkPuzzleScript.PuzzlesCompleted.ContainsKey(TurkPuzzleScript.CurrentDifficutly)) return;
        int puzzlesCompleted = TurkPuzzleScript.PuzzlesCompleted[TurkPuzzleScript.CurrentDifficutly];
        if (!currentDifficultyDialogue.SolutionDialogues.ContainsKey(puzzlesCompleted)) return;

        VisionCompletionMascotText textData = currentDifficultyDialogue.SolutionDialogues[puzzlesCompleted];

        if (textData.Triggered) return;
        textData.Triggered = true;
        MascotSayText(textData.SolutionDialogues);
    }

    public void OnPuzzleEnd()
    {
        foreach(Coroutine c in WaitCoroutines)
        {
            if(c != null) StopCoroutine(c);
        }
    }

    public void OnNameSet(string newName)
    {
        NameText.text = newName;
        WaitForInteraction = false;
        WaitForText = false;
        SkipWait = true;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!EnableProgress) return;
        if (!context.started) return;

        if (WaitForInteraction) return;

        if (TextChainActive)
        {
            WaitForText = false;
            return;
        }

        MascotClearText();
    }

    public void OnAdClose()
    {
        int currentDifficulty = TurkPuzzleScript.CurrentDifficutly;
        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];
        MascotSayText(currentDifficultyDialogue.SpamCloseDialogues[currentDifficultyDialogue.SpamCloseOccurances % currentDifficultyDialogue.SpamCloseDialogues.Count]);
        currentDifficultyDialogue.SpamCloseOccurances++;
    }

    public void OnMascotClick()
    {
        if (DialogueActive) return;

        int currentDifficulty = TurkPuzzleScript.CurrentDifficutly;
        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];
        MascotSayText(currentDifficultyDialogue.ClickDialogues[currentDifficultyDialogue.ClickOccurrences % currentDifficultyDialogue.ClickDialogues.Count]);
        currentDifficultyDialogue.ClickOccurrences++;
    }

    public static void OnSubmitChoice(string text)
    {
        instance.MascotOnSubmitChoice(text);
    }
    public void MascotOnSubmitChoice(string text)
    {
        WaitForInteraction = false;
        ShowText(text);
    }

    public void OnDifficultyIncrease(int currentDifficulty)
    {
        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];
        SpeechAudioSource.clip = currentDifficultyDialogue.SpeechSound;
        if (currentDifficultyDialogue.FirstIncrease)
        {
            currentDifficultyDialogue.FirstIncrease = false;
            MascotSayText(currentDifficultyDialogue.FirstDifficultyIncreaseDialogues);
            return;
        }
        MascotSayText(currentDifficultyDialogue.DifficultyIncreaseDialogues[currentDifficultyDialogue.IncreaseOccurrences % currentDifficultyDialogue.DifficultyIncreaseDialogues.Count]);
        currentDifficultyDialogue.IncreaseOccurrences++;
    }

    public void OnDifficultyDecrease(int currentDifficulty)
    {
        if (currentDifficulty >= DifficultyChangeMessages.Count) return;

        MascotDifficultyDialogueSO currentDifficultyDialogue = DifficultyChangeMessages[currentDifficulty];
        SpeechAudioSource.clip = currentDifficultyDialogue.SpeechSound;
        if (currentDifficultyDialogue.FirstDecrease)
        {
            currentDifficultyDialogue.FirstDecrease = false;
            MascotSayText(currentDifficultyDialogue.FirstDifficultyDecreaseDialogues);
            return;
        }
        MascotSayText(currentDifficultyDialogue.DifficultyDecreaseDialogues[currentDifficultyDialogue.DecreaseOccurrences % currentDifficultyDialogue.DifficultyDecreaseDialogues.Count]);
        currentDifficultyDialogue.DecreaseOccurrences++;
    }
}
