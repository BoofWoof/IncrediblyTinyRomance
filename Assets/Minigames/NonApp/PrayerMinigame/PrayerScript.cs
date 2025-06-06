using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class PrayerScript : MonoBehaviour
{
    public static PrayerScript instance;

    public TextAsset GoodPrayers;
    private string[] GoodLines = null;
    public TextAsset BadPrayers;
    private string[] BadLines = null;

    private float RamAngyLevel = 0;
    public float AngerRate = 1f;

    private int GoodIdx;

    public List<Button> ButtonText;
    public List<TMP_Text> AuthorText;
    public TMP_Text AngyDebug;

    public delegate void PrayerSubmittedCallback(bool GoodPrayer);
    public static event PrayerSubmittedCallback PrayerSubmitted;

    public static int GoodPrayerCount = 0;
    public static int BadPrayerCount = 0;
    public static int TotalPrayerCount = 0;

    public static bool IncreaseAnger = false;

    public float WaitForNextPrayerSec = 2f;

    public SplineMessageScript FireworkLauncher;
    public PrayerFireworkTextScript PrayerFireworkTextScript;

    //Dialogue Options
    public static bool StoryMode = false;
    private Response[] Responses;

    private void Start()
    {
        instance = this;

        ProcessPrayers();
        GenerateNewPrayers();
    }

    private void Update()
    {
        if (!IncreaseAnger) return;
        RamAngyLevel += Time.deltaTime * AngerRate;
        AngyDebug.text = "RamAngyLevel: " + RamAngyLevel.ToString("0");
    }
    public void SubmitAnswer(int answerIdx)
    {
        if (StoryMode)
        {
            StartCoroutine(SubmitResponse(answerIdx));
        } else
        {
            StartCoroutine(SubmitPrayer(answerIdx));
        }
    }

    #region Conversation
    public IEnumerator SubmitResponse(int answerIdx)
    {
        DisableButtons();
        AuthorText[answerIdx].text = "";

        int actualAnswerIdx = 0;

        if (Responses.Length == 1)
        {
            actualAnswerIdx = 0;
        }
        else if (Responses.Length == 2)
        {
            if (answerIdx == 2)
            {
                actualAnswerIdx = 1;
            }
            else
            {
                actualAnswerIdx = 0;
            }
        }
        else if (Responses.Length == 3)
        {
            actualAnswerIdx = answerIdx;
        }

        FireworkLauncher.ActivateMessage();
        string answerText = Responses[actualAnswerIdx].formattedText.text;
        PrayerFireworkTextScript.ActivateFirework(answerText);

        yield return new WaitForSeconds(WaitForNextPrayerSec + 3f);
        (DialogueManager.dialogueUI as AbstractDialogueUI).OnClick(Responses[actualAnswerIdx]);
    }
    public void OnConversationResponseMenu(Response[] responses)
    {
        if (!ConversationManagerScript.isMacroConvo) return;
        Debug.Log("Creating choices: " + responses.Length.ToString());

        Responses = responses;

        for (int i = 0; i < ButtonText.Count; i++)
        {
            ButtonText[i].GetComponent<MessageSendScript>().RestartMessage();
        }
        if (responses.Length == 1)
        {
            ButtonText[0].GetComponentInChildren<TMP_Text>().text = "";
            ButtonText[0].interactable = false;
            ButtonText[1].GetComponentInChildren<TMP_Text>().text = responses[0].formattedText.text;
            ButtonText[1].interactable = true;
            ButtonText[2].GetComponentInChildren<TMP_Text>().text = "";
            ButtonText[2].interactable = false;
        } 
        else if (responses.Length == 2)
        {
            ButtonText[0].GetComponentInChildren<TMP_Text>().text = responses[0].formattedText.text;
            ButtonText[0].interactable = true;
            ButtonText[1].GetComponentInChildren<TMP_Text>().text = "";
            ButtonText[1].interactable = false;
            ButtonText[2].GetComponentInChildren<TMP_Text>().text = responses[1].formattedText.text;
            ButtonText[2].interactable = true;
        }
        else if (responses.Length == 3)
        {
            ButtonText[0].GetComponentInChildren<TMP_Text>().text = responses[0].formattedText.text;
            ButtonText[0].interactable = true;
            ButtonText[1].GetComponentInChildren<TMP_Text>().text = responses[1].formattedText.text;
            ButtonText[1].interactable = true;
            ButtonText[2].GetComponentInChildren<TMP_Text>().text = responses[2].formattedText.text;
            ButtonText[2].interactable = true;
        }
    }
    public void OnConversationEnd(Transform actor)
    {
        if (!ConversationManagerScript.isMacroConvo) return;
        StoryMode = false;
        GenerateNewPrayers();
        MusicSelectorScript.SetOverworldSong(1);
    }
    #endregion

    #region MiniGame
    IEnumerator SubmitPrayer(int answerIdx)
    {
        int allCount = DialogueLua.GetVariable("PrayersSubmitted").asInt;
        DialogueLua.SetVariable("PrayersSubmitted", allCount + 1);

        FireworkLauncher.ActivateMessage();
        string answerText = ButtonText[answerIdx].GetComponentInChildren<TMP_Text>().text;
        PrayerFireworkTextScript.ActivateFirework(answerText);

        TotalPrayerCount += 1;
        if (GoodIdx == answerIdx)
        {
            int correctCount = DialogueLua.GetVariable("SuccessfulPrayersSubmitted").asInt;
            DialogueLua.SetVariable("SuccessfulPrayersSubmitted", correctCount + 1);
            Debug.Log("You win!");
            RamAngyLevel -= 30f;
            if (RamAngyLevel < 0) RamAngyLevel = 0;
            PrayerSubmitted.Invoke(true);
            GoodPrayerCount++;
        }
        else
        {
            int failCount = DialogueLua.GetVariable("FailedPrayersSubmitted").asInt;
            DialogueLua.SetVariable("FailedPrayersSubmitted", failCount + 1);
            Debug.Log("Ram be angy! >:C");
            RamAngyLevel += 30f;
            if (RamAngyLevel > 100) RamAngyLevel = 100;
            PrayerSubmitted.Invoke(false);
            BadPrayerCount++;
        }

        DisableButtons();
        AuthorText[answerIdx].text = "";

        yield return new WaitForSeconds(WaitForNextPrayerSec);

        GenerateNewPrayers();
    }

    private void DisableButtons()
    {
        //DisableButtons
        for (int i = 0; i < ButtonText.Count; i++)
        {
            ButtonText[i].interactable = false;
        }
    }

    private void ProcessPrayers()
    {
        GoodLines = GoodPrayers.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        if (GoodLines.Length < 1)
        {
            Debug.LogError("Not enough lines in the text file!");
            return;
        }
        BadLines = BadPrayers.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
        // Ensure we have at least 3 lines to select from
        if (BadLines.Length < 3)
        {
            Debug.LogError("Not enough lines in the text file!");
            return;
        }
    }

    private void GenerateNewPrayers()
    {
        if (StoryMode) return;

        List<string> selectedGoodLine = GetRandomUniqueLines(GoodLines, 1);
        List<string> selectedBadLines = GetRandomUniqueLines(BadLines, 2);

        GoodIdx = Random.Range(0, 3);
        int badCount = 0;
        for (int i = 0; i < ButtonText.Count; i++)
        {
            string[] split;
            if (GoodIdx == i)
            {
                split = selectedGoodLine[0].Split(" @");
                ButtonText[i].GetComponentInChildren<TMP_Text>().text = split[0];
                AuthorText[i].text = split[1];
                continue;
            }
            split = selectedBadLines[badCount].Split(" @");
            ButtonText[i].GetComponentInChildren<TMP_Text>().text = split[0];
            AuthorText[i].text = split[1];
            badCount++;
        }

        for (int i = 0; i < ButtonText.Count; i++)
        {
            ButtonText[i].GetComponent<MessageSendScript>().RestartMessage();
        }
    }

    private List<string> GetRandomUniqueLines(string[] lines, int count)
    {
        // Create a list to hold unique lines
        HashSet<int> selectedIndices = new HashSet<int>();
        List<string> result = new List<string>();

        // Randomly select unique indices
        while (selectedIndices.Count < count)
        {
            int randomIndex = Random.Range(0, lines.Length);
            if (selectedIndices.Add(randomIndex)) // Add returns false if already present
            {
                result.Add(lines[randomIndex]);
            }
        }

        return result;
    }
    #endregion
}
