using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using System.Linq;

public class PrayerScript : MonoBehaviour
{
    public static PrayerScript instance;

    public TextAsset GoodPrayers;
    private string[] GoodLines = null;
    public TextAsset BadPrayers;
    private string[] BadLines = null;

    public float RamAngyLevel = 0;
    public float AngerRate = 1f;
    public float AngerReduction = 50f;
    public float AngerThreshold = 180f;

    private int GoodIdx;

    public List<MessageSendScript> SubmissionButtons;
    public TMP_Text AngyDebug;

    public delegate void PrayerSubmittedCallback(bool GoodPrayer);
    public static event PrayerSubmittedCallback PrayerSubmitted;

    public static int GoodPrayerCount = 0;
    public static int BadPrayerCount = 0;
    public static int TotalPrayerCount = 0;

    public float WaitForNextPrayerSec = 2f;

    public SplineMessageScript FireworkLauncher;
    public PrayerFireworkTextScript PrayerFireworkTextScript;

    //Dialogue Options
    public static bool StoryMode = false;
    private Response[] Responses;

    [Header("Judgement Variables")]
    public bool JudgementActive = false;
    public bool JudgementFocus = false;

    public List<VoiceLineSO> PositiveJudgementAudios;
    public List<int> LastPositiveJudgementIdxs;
    public int UniquePositivesTillRepeatAllowed;
    public List<VoiceLineSO> NegativeJudgementAudios;
    public List<int> LastNegativeJudgementIdxs;
    public int UniqueNegativesTillRepeatAllowed;

    public SpecialPrayerData[] SetSpecialPrayers = new SpecialPrayerData[3];

    public void ActivateJudgement()
    {
        Debug.Log("Activating Judgement");
        JudgementActive = true;
        RamAngyLevel = 0;
        JudgementFocus = false;
    }

    public void DeactivateJudgement()
    {
        Debug.Log("Deactivating Judgement");
        JudgementActive = false;
        JudgementFocus = false;
    }

    public void IncreaseAngerThreshold(float AdditionalThreshold)
    {
        AngerThreshold += AdditionalThreshold;
    }

    public float GetAngerLevel()
    {
        return (RamAngyLevel / AngerThreshold);
    }

    private void Update()
    {
        if (!JudgementActive) return;
        RamAngyLevel += Time.deltaTime * AngerRate;
        AngyDebug.text = "RamAngyLevel: " + GetAngerLevel().ToString("F2");

        if(!JudgementFocus && GetAngerLevel() > 0.8f)
        {
            CharacterSpeechScript.BroadcastGestureParameter("MacroAries", "SitForward");
            JudgementFocus = true;
        }

        if (JudgementFocus && GetAngerLevel() < 0.4f)
        {
            CharacterSpeechScript.BroadcastGestureParameter("MacroAries", "SitBack");
            JudgementFocus = false;
        }
    }

    private void Start()
    {
        instance = this;

        ProcessPrayers();
        GenerateNewPrayers();

        SPrayerSubmissionScript.OnNewForcedPrayers += OnNewForcedPrayer;
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
        SubmissionButtons[answerIdx].SetAuthorName("");

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

        for (int i = 0; i < 3; i++)
        {
            SubmissionButtons[i].RestartMessage();
            SubmissionButtons[i].SetAuthorName("");
        }

        List<string> stringResponse = new List<string>();
        foreach (Response response in responses)
        {
            stringResponse.Add(response.formattedText.text);
        }
        CreateVariableLenghtOptions(stringResponse);
    }

    public void CreateVariableLenghtOptions(List<string> responses, List<string> authors = null)
    {
        if (responses.Count == 1)
        {
            SubmissionButtons[0].SetButtonText("");
            SubmissionButtons[0].SubmissionButton.interactable = false;
            SubmissionButtons[0].SetNormalResponse();
            SubmissionButtons[1].SetButtonText(responses[0]);
            SubmissionButtons[1].SubmissionButton.interactable = true;
            SubmissionButtons[1].SetSpecialResponse();
            SubmissionButtons[2].SetButtonText("");
            SubmissionButtons[2].SubmissionButton.interactable = false;
            SubmissionButtons[2].SetNormalResponse();
            if(authors != null)
            {
                SubmissionButtons[1].SetAuthorName(authors[0]);
            }
        }
        else if (responses.Count == 2)
        {
            SubmissionButtons[0].SetButtonText(responses[0]);
            SubmissionButtons[0].SubmissionButton.interactable = true;
            SubmissionButtons[0].SetSpecialResponse();
            SubmissionButtons[1].SetButtonText("");
            SubmissionButtons[1].SubmissionButton.interactable = false;
            SubmissionButtons[1].SetNormalResponse();
            SubmissionButtons[2].SetButtonText(responses[1]);
            SubmissionButtons[2].SubmissionButton.interactable = true;
            SubmissionButtons[2].SetSpecialResponse();
            if (authors != null)
            {
                SubmissionButtons[0].SetAuthorName(authors[0]);
                SubmissionButtons[2].SetAuthorName(authors[1]);
            }
        }
        else if (responses.Count == 3)
        {
            SubmissionButtons[0].SetButtonText(responses[0]);
            SubmissionButtons[0].SubmissionButton.interactable = true;
            SubmissionButtons[0].SetSpecialResponse();
            SubmissionButtons[1].SetButtonText(responses[1]);
            SubmissionButtons[1].SubmissionButton.interactable = true;
            SubmissionButtons[1].SetSpecialResponse();
            SubmissionButtons[2].SetButtonText(responses[2]);
            SubmissionButtons[2].SubmissionButton.interactable = true;
            SubmissionButtons[2].SetSpecialResponse();
            if (authors != null)
            {
                SubmissionButtons[0].SetAuthorName(authors[0]);
                SubmissionButtons[1].SetAuthorName(authors[1]);
                SubmissionButtons[2].SetAuthorName(authors[2]);
            }
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
        FireworkLauncher.ActivateMessage();
        string answerText = SubmissionButtons[answerIdx].SubmissionButton.GetComponentInChildren<TMP_Text>().text;
        PrayerFireworkTextScript.ActivateFirework(answerText);

        SubmissionButtons[answerIdx].SetButtonText("");
        SubmissionButtons[answerIdx].SetAuthorName("");
        DisableButtons();

        yield return new WaitForSeconds(5f);

        TotalPrayerCount += 1;
        if (GoodIdx == answerIdx || SubmissionButtons[answerIdx].IsSpecial)
        {
            Debug.Log("You win!");
            RamAngyLevel -= AngerReduction;
            if (RamAngyLevel < 0) RamAngyLevel = 0;
            PrayerSubmitted.Invoke(true);
            GoodPrayerCount++;

            int RandomIdx = Random.Range(0, PositiveJudgementAudios.Count);
            
            if(SetSpecialPrayers[answerIdx].SpecialResponseChain != null && SetSpecialPrayers[answerIdx].SpecialResponseChain.Count > 0)
            {
                SPrayerSubmissionScript.WaitingSpecialPrayers.Remove(SetSpecialPrayers[answerIdx]);
                CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", SetSpecialPrayers[answerIdx].SpecialResponseChain);
            } else
            {
                if (JudgementActive) CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", PositiveJudgementAudios[RandomIdx]);
            }
        }
        else
        {
            Debug.Log("Ram be angy! >:C");
            RamAngyLevel += AngerReduction * 2f / 3f;
            if (RamAngyLevel > AngerThreshold)
            {
                RamAngyLevel = AngerThreshold;
                ActivateGameOver();
            }
            PrayerSubmitted.Invoke(false);
            BadPrayerCount++;

            int RandomIdx = Random.Range(0, NegativeJudgementAudios.Count);
            if (JudgementActive) CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", NegativeJudgementAudios[RandomIdx]);
        }

        yield return new WaitForSeconds(WaitForNextPrayerSec);

        GenerateNewPrayers();
    }

    private void ActivateGameOver()
    {
        OverworldBehavior.BroadcastBehaviors("A", "Tap");
    }

    private void DisableButtons()
    {
        //DisableButtons
        for (int i = 0; i < 3; i++)
        {
            SubmissionButtons[i].SetButtonText("");
            SubmissionButtons[i].SetAuthorName("");
            SubmissionButtons[i].SetNormalResponse();
            SubmissionButtons[i].SubmissionButton.interactable = false;
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

    public void OnNewForcedPrayer()
    {
        GenerateNewPrayers();
    }

    private void GenerateNewPrayers()
    {
        if (StoryMode) return;

        SetSpecialPrayers = new SpecialPrayerData[3];
        if (SPrayerSubmissionScript.WaitingForcedPrayers.Count > 0)
        {
            SpecialPrayerSetSO targetPrayer = SPrayerSubmissionScript.WaitingForcedPrayers[0];
            SPrayerSubmissionScript.WaitingForcedPrayers.Remove(targetPrayer);

            List<string> PrayerText = new List<string>();
            foreach(SpecialPrayerData prayerData in targetPrayer.PrayerOptions)
            {
                PrayerText.Add(prayerData.Option);
            }
            CreateVariableLenghtOptions(PrayerText);

            if (targetPrayer.PrayerOptions.Length == 1)
            {
                SetSpecialPrayers[1] = targetPrayer.PrayerOptions[0];
            }
            else if (targetPrayer.PrayerOptions.Length == 2)
            {
                SetSpecialPrayers[0] = targetPrayer.PrayerOptions[0];
                SetSpecialPrayers[2] = targetPrayer.PrayerOptions[1];
            }
            else
            {
                SetSpecialPrayers[0] = targetPrayer.PrayerOptions[0];
                SetSpecialPrayers[1] = targetPrayer.PrayerOptions[1];
                SetSpecialPrayers[2] = targetPrayer.PrayerOptions[2];
            }

            RestartMessages();
            return;
        }

        List<string> selectedGoodLine = GetRandomUniqueLines(GoodLines, 1);
        List<string> selectedBadLines = GetRandomUniqueLines(BadLines, 2);

        GoodIdx = Random.Range(0, 3);
        int badCount = 0;
        for (int i = 0; i < 3; i++)
        {
            int SpecialPrayerCount = SPrayerSubmissionScript.WaitingSpecialPrayers.Count;
            if (SPrayerSubmissionScript.WaitingSpecialPrayers.Count > 0)
            {
                if(Random.value < 0.9f)
                {
                    SpecialPrayerData selectedPrayer = SPrayerSubmissionScript.WaitingSpecialPrayers[Random.Range(0, SpecialPrayerCount)];

                    if (!SetSpecialPrayers.Contains(selectedPrayer))
                    {
                        SubmissionButtons[i].SetButtonText(selectedPrayer.Option);
                        SubmissionButtons[i].SetAuthorName(selectedPrayer.AuthorName);
                        SubmissionButtons[i].SetSpecialResponse();
                        SetSpecialPrayers[i] = selectedPrayer;

                        continue;
                    }
                }
            }


            string[] split;
            if (GoodIdx == i)
            {
                split = selectedGoodLine[0].Split(" @");
                SubmissionButtons[i].SetButtonText(split[0]);
                SubmissionButtons[i].SetAuthorName(split[1]);
                SubmissionButtons[i].SetNormalResponse();
                continue;
            }
            split = selectedBadLines[badCount].Split(" @");
            SubmissionButtons[i].SetButtonText(split[0]);
            SubmissionButtons[i].SetAuthorName(split[1]);
            SubmissionButtons[i].SetNormalResponse();
            badCount++;
        }
        RestartMessages();
    }

    public void RestartMessages()
    {
        for (int i = 0; i < 3; i++)
        {
            SubmissionButtons[i].RestartMessage();
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
