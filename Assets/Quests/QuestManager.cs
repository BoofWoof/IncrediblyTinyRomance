using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static TMP_Text QuestText;
    public TMP_Text questText;

    public static string currentQuest;
    public static int currentQuestIndex;

    public delegate void QuestChangedDelegate(string newQuest);
    public static QuestChangedDelegate NewQuest;
    public static QuestChangedDelegate CompletedQuest;

    public static QuestManager QuestManagerInstance;

    public float CharacterPeriod = 0.005f;
    public Coroutine TransitionCoroutine;

    private void Awake()
    {
        QuestText = questText;
        currentQuestIndex = 0;

        QuestManagerInstance = this;
        transform.parent = DialogueManager.instance.transform;
    }

    private void Start()
    {
        QuestText.text = "";

        currentQuestIndex = 0;
        string[] QuestList = QuestLog.GetAllQuests(QuestState.Active | QuestState.Success | QuestState.Unassigned, false);
        foreach (string questName in QuestList)
        {
            if (QuestLog.GetQuestState(questName) == QuestState.Active)
            {
                currentQuest = questName;
                UpdateText(questName);
                break;
            }
            currentQuestIndex++;
        }

        Lua.RegisterFunction("IncrementQuest", this, SymbolExtensions.GetMethodInfo(() => IncrementQuest()));
    }

    public static void CompleteQuest(string questTitle)
    {
        QuestLog.SetQuestState(questTitle, QuestState.Success);
        QuestText.color = Color.green;
    }

    public static void ChangeQuest (string newQuestTitle)
    {
        QuestText.color = Color.white;
        string[] QuestList = QuestLog.GetAllQuests(QuestState.Active | QuestState.Success | QuestState.Unassigned, false);
        currentQuestIndex = Array.IndexOf(QuestList, newQuestTitle);

        QuestLog.SetQuestState(currentQuest, QuestState.Success);
        QuestLog.SetQuestState(newQuestTitle, QuestState.Active);


        CompletedQuest?.Invoke(currentQuest);

        currentQuest = newQuestTitle;

        NewQuest?.Invoke(currentQuest);
        QuestManagerInstance.UpdateText(currentQuest);
    }

    public static void IncrementQuest()
    {
        string[] QuestList = QuestLog.GetAllQuests(QuestState.Active|QuestState.Success|QuestState.Unassigned, false);

        string newQuest = QuestList[currentQuestIndex + 1];

        ChangeQuest(newQuest);
    }

    public static void SetQuestByIndex(int QuestIdx)
    {
        string[] QuestList = QuestLog.GetAllQuests(QuestState.Active | QuestState.Success | QuestState.Unassigned, false);

        string newQuest = QuestList[QuestIdx];

        ChangeQuest(newQuest);
    }

    public void UpdateText(string questTitle)
    {
        string title = QuestLog.GetQuestTitle(questTitle);
        string description = QuestLog.GetQuestDescription(questTitle);
        FormattedText formattedDescription = FormattedText.Parse(description);

        Debug.Log(title.ToUpper() + ": " + formattedDescription.text);

        if(TransitionCoroutine != null) StopCoroutine(TransitionCoroutine);

        TransitionCoroutine = StartCoroutine(TransitionText(title.ToUpper() + ": " + formattedDescription.text));
    }

    public void QuickUpdate()
    {
        string title = QuestLog.GetQuestTitle(currentQuest);
        string description = QuestLog.GetQuestDescription(currentQuest);
        FormattedText formattedDescription = FormattedText.Parse(description);

        string newDescription = title.ToUpper() + ": " + formattedDescription.text;
        QuestText.text = newDescription;
    }

    public void OnQuestStateChange(string questName)
    {
    }

    public void OnQuestEntryStateChange(QuestEntryArgs args)
    {
    }

    public IEnumerator TransitionText(string newText)
    {
        while (QuestText.text.Length > 0)
        {
            QuestText.text = QuestText.text.Substring(0, QuestText.text.Length - 1);
            if (UnityEngine.Random.Range(0f, 1f) > 0.8f)
            {
                yield return new WaitForSeconds(CharacterPeriod);
            }
        }

        yield return new WaitForSeconds(0.1f);

        int lettersRevealed = 1;
        while (QuestText.text.Length != newText.Length)
        {
            QuestText.text = newText.Substring(0, lettersRevealed);
            if (UnityEngine.Random.Range(0f, 1f) > 0.6f) {
                yield return new WaitForSeconds(CharacterPeriod);
            } 
            lettersRevealed++;
        }
        QuickUpdate();
    }
}
