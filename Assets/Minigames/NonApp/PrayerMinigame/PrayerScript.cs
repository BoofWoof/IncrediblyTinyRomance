using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.Events;

public class PrayerResponse
{
    public int AssociatedIdx;
    public string Prayer;
    public string Author;
    public bool GoodPrayer;
    public bool SpecialPrayer;
    
    public Response AssociatedResponse;

    public SpecialPrayerData AssociatedSpecialPrayerData;
    public SpecialPrayerSetSO AssociatedSpecialPrayerSet;
}


public class PrayerScript : MonoBehaviour
{
    public UnityEvent OnGoodPrayer;
    public UnityEvent OnBadPrayer;
    public UnityEvent OnPrayer;

    public static PrayerScript instance;

    public Dictionary<int, PrayerResponse> CurrentResponse;

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

    public UnityEvent OnJudgementActivate;

    private bool _SpecialPrayerActive = false;

    public void ActivateJudgement()
    {
        if (JudgementActive) return;
        Debug.Log("Activating Judgement");
        JudgementActive = true;
        RamAngyLevel = 0;
        JudgementFocus = false;
        BalconyEventsScript.instance.StartSystem();

        GenerateNewPrayers();

        OnJudgementActivate?.Invoke();
    }

    public void DeactivateJudgement()
    {
        if (!JudgementActive) return;
        Debug.Log("Deactivating Judgement");
        JudgementActive = false;
        JudgementFocus = false;
        BalconyEventsScript.instance.StopSystem();
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
        if (RamAngyLevel > AngerThreshold)
        {
            RamAngyLevel = AngerThreshold;
            ActivateGameOver();
        }
    }

    private void Start()
    {
        instance = this;

        ProcessPrayers();
        GenerateNewPrayers();

        SPrayerSubmissionScript.OnNewForcedPrayers += OnNewForcedPrayer;
        GameStateMonitor.OnEventChange += OnGameEventStateChange;
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

        FireworkLauncher.ActivateMessage();
        string answerText = CurrentResponse[answerIdx].AssociatedResponse.formattedText.text;
        PrayerFireworkTextScript.ActivateFirework(answerText);

        yield return new WaitForSeconds(WaitForNextPrayerSec + 3f);
        (DialogueManager.dialogueUI as AbstractDialogueUI).OnClick(CurrentResponse[answerIdx].AssociatedResponse);
    }
    public void OnConversationResponseMenu(Response[] responses)
    {
        if (!ConversationManagerScript.isMacroConvo) return;

        BalconyEventsScript.instance.StartSystem();

        Debug.Log("Creating choices: " + responses.Length.ToString());

        Responses = responses;

        for (int i = 0; i < 3; i++)
        {
            SubmissionButtons[i].RestartMessage();
            SubmissionButtons[i].SetAuthorName("");
        }

        List<PrayerResponse> prayerResponses = new List<PrayerResponse>();
        foreach (Response response in responses)
        {
            PrayerResponse newResponse = new PrayerResponse
            {
                AssociatedIdx = -1,
                Prayer = response.formattedText.text,
                Author = "SozaPM",
                GoodPrayer = true,
                SpecialPrayer = false,
                AssociatedResponse = response
            };

            prayerResponses.Add(newResponse);
        }
        CreateVariableLengthOptions(prayerResponses);
    }

    public void CreateVariableLengthOptions(List<PrayerResponse> responses)
    {
        Debug.Log($"Prayer Count: {responses.Count}");

        if (responses.Count == 1)
        {
            SubmissionButtons[0].gameObject.SetActive(false);
            SubmissionButtons[0].SetButtonText("");
            SubmissionButtons[0].SubmissionButton.interactable = false;
            SubmissionButtons[0].SetNormalResponse();
            SubmissionButtons[0].SetAuthorName("");

            SubmissionButtons[1].gameObject.SetActive(true);
            SubmissionButtons[1].SetButtonText(responses[0].Prayer);
            SubmissionButtons[1].SubmissionButton.interactable = true;
            if (responses[0].SpecialPrayer) SubmissionButtons[1].SetSpecialResponse(responses[0].GoodPrayer);
            else SubmissionButtons[1].SetNormalResponse();
            SubmissionButtons[1].SetAuthorName(responses[0].Author);
            responses[0].AssociatedIdx = 1;

            SubmissionButtons[2].gameObject.SetActive(false);
            SubmissionButtons[2].SetButtonText("");
            SubmissionButtons[2].SubmissionButton.interactable = false;
            SubmissionButtons[2].SetNormalResponse();
            SubmissionButtons[2].SetAuthorName("");
        }
        else if (responses.Count == 2)
        {
            SubmissionButtons[0].gameObject.SetActive(true);
            SubmissionButtons[0].SetButtonText(responses[0].Prayer);
            SubmissionButtons[0].SubmissionButton.interactable = true;
            if (responses[0].SpecialPrayer) SubmissionButtons[0].SetSpecialResponse(responses[0].GoodPrayer);
            else SubmissionButtons[0].SetNormalResponse();
            SubmissionButtons[0].SetAuthorName(responses[0].Author);
            responses[0].AssociatedIdx = 0;

            SubmissionButtons[1].gameObject.SetActive(false);
            SubmissionButtons[1].SetButtonText("");
            SubmissionButtons[1].SubmissionButton.interactable = false;
            SubmissionButtons[1].SetNormalResponse();
            SubmissionButtons[1].SetAuthorName("");

            SubmissionButtons[2].gameObject.SetActive(true);
            SubmissionButtons[2].SetButtonText(responses[1].Prayer);
            SubmissionButtons[2].SubmissionButton.interactable = true;
            if (responses[1].SpecialPrayer) SubmissionButtons[2].SetSpecialResponse(responses[1].GoodPrayer);
            else SubmissionButtons[2].SetNormalResponse();
            SubmissionButtons[2].SetAuthorName(responses[1].Author);
            responses[1].AssociatedIdx = 2;
        }
        else if (responses.Count == 3)
        {
            SubmissionButtons[0].gameObject.SetActive(true);
            SubmissionButtons[0].SetButtonText(responses[0].Prayer);
            SubmissionButtons[0].SubmissionButton.interactable = true;
            if (responses[0].SpecialPrayer) SubmissionButtons[0].SetSpecialResponse(responses[0].GoodPrayer);
            else SubmissionButtons[0].SetNormalResponse();
            SubmissionButtons[0].SetAuthorName(responses[0].Author);
            responses[0].AssociatedIdx = 0;

            SubmissionButtons[1].gameObject.SetActive(true);
            SubmissionButtons[1].SetButtonText(responses[1].Prayer);
            SubmissionButtons[1].SubmissionButton.interactable = true;
            if (responses[1].SpecialPrayer) SubmissionButtons[1].SetSpecialResponse(responses[1].GoodPrayer);
            else SubmissionButtons[1].SetNormalResponse();
            SubmissionButtons[1].SetAuthorName(responses[1].Author);
            responses[1].AssociatedIdx = 1;

            SubmissionButtons[2].gameObject.SetActive(true);
            SubmissionButtons[2].SetButtonText(responses[2].Prayer);
            SubmissionButtons[2].SubmissionButton.interactable = true;
            if (responses[2].SpecialPrayer) SubmissionButtons[2].SetSpecialResponse(responses[2].GoodPrayer);
            else SubmissionButtons[2].SetNormalResponse();
            SubmissionButtons[2].SetAuthorName(responses[2].Author);
            responses[2].AssociatedIdx = 2;
        }
        CurrentResponse = new Dictionary<int, PrayerResponse>();
        foreach (PrayerResponse response in responses)
        {
            CurrentResponse.Add(response.AssociatedIdx, response);
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
        GameStateMonitor.ActivePrayer = true;

        bool isSpecialPrayer = CurrentResponse[answerIdx].SpecialPrayer;

        FireworkLauncher.ActivateMessage();
        string answerText = CurrentResponse[answerIdx].Prayer;
        PrayerFireworkTextScript.ActivateFirework(answerText);

        SubmissionButtons[answerIdx].SetButtonText("");
        SubmissionButtons[answerIdx].SetAuthorName("");
        DisableButtons();


        if (isSpecialPrayer)
        {
            CurrentResponse[answerIdx].AssociatedSpecialPrayerData.StartLoadResponseChain();
        }

        yield return new WaitForSeconds(5f);

        OnPrayer?.Invoke();

        TotalPrayerCount++;
        if (CurrentResponse[answerIdx].GoodPrayer)
        {
            GoodPrayerCount++;
            OnGoodPrayer?.Invoke();
            PrayerSubmitted.Invoke(true);
        } else
        {
            BadPrayerCount++;
            OnBadPrayer?.Invoke();
            PrayerSubmitted.Invoke(false);
        }

        if (isSpecialPrayer)
        {
            RamAngyLevel -= AngerReduction;
            if (RamAngyLevel < 0) RamAngyLevel = 0;

            yield return instance.StartCoroutine(CurrentResponse[answerIdx].AssociatedSpecialPrayerData.WaitLoadResponseChain());

            if (CurrentResponse[answerIdx].AssociatedSpecialPrayerData.SpecialResponseChainVL != null && CurrentResponse[answerIdx].AssociatedSpecialPrayerData.SpecialResponseChainVL.Count > 0)
            {
                if (CurrentResponse[answerIdx].AssociatedSpecialPrayerSet != null) SPrayerSubmissionScript.WaitingForcedPrayers.Remove(CurrentResponse[answerIdx].AssociatedSpecialPrayerSet);
                else SPrayerSubmissionScript.WaitingSpecialPrayers.Remove(CurrentResponse[answerIdx].AssociatedSpecialPrayerData);

                CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", CurrentResponse[answerIdx].AssociatedSpecialPrayerData.SpecialResponseChainVL);

                yield return new WaitForSeconds(CurrentResponse[answerIdx].AssociatedSpecialPrayerData.GetChainTime() + 0.1f);
            }

        } else if (GoodIdx == answerIdx)
        {

            RamAngyLevel -= AngerReduction;
            if (RamAngyLevel < 0) RamAngyLevel = 0;

            int RandomIdx = Random.Range(0, PositiveJudgementAudios.Count);
            if (JudgementActive) CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", PositiveJudgementAudios[RandomIdx]);
            yield return new WaitForSeconds(PositiveJudgementAudios[RandomIdx].AudioData.length + 0.1f);
        }
        else
        {
            RamAngyLevel += AngerReduction * 2f / 3f;
            if (RamAngyLevel > AngerThreshold)
            {
                GameStateMonitor.ActivePrayer = false;
                yield break;
            }

            int RandomIdx = Random.Range(0, NegativeJudgementAudios.Count);
            if (JudgementActive) CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", NegativeJudgementAudios[RandomIdx]);
            yield return new WaitForSeconds(NegativeJudgementAudios[RandomIdx].AudioData.length + 0.1f);
        }


        GameStateMonitor.ActivePrayer = false;
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
        if (GameStateMonitor.isEventActive()) return;
        GenerateNewPrayers();
    }
    public void OnGameEventStateChange(bool EventActive)
    {
        if (SPrayerSubmissionScript.WaitingForcedPrayers.Count > 0 || _SpecialPrayerActive) GenerateNewPrayers();
    }

    public void ForcePrayerReferesh()
    {
        GenerateNewPrayers();
    }

    private void GenerateNewPrayers()
    {
        if (StoryMode) return;
        if (GameStateMonitor.ActivePrayer) return;

        if (SPrayerSubmissionScript.WaitingForcedPrayers.Count > 0 && !GameStateMonitor.isEventActive())
        {
            Debug.Log("Generating Special Prayer Set");
            SpecialPrayerSetSO targetPrayer = SPrayerSubmissionScript.WaitingForcedPrayers[0];

            List<PrayerResponse> prayerResponsesSpecial = new List<PrayerResponse>();
            foreach(SpecialPrayerData prayerData in targetPrayer.PrayerOptions)
            {
                PrayerResponse newResponse = new PrayerResponse
                {
                    AssociatedIdx = -1,
                    Prayer = prayerData.Option,
                    Author = prayerData.AuthorName,
                    GoodPrayer = prayerData.GoodPrayer,
                    SpecialPrayer = true,
                    AssociatedSpecialPrayerData = prayerData,
                    AssociatedSpecialPrayerSet = targetPrayer
                };

                prayerResponsesSpecial.Add(newResponse);
            }
            CreateVariableLengthOptions(prayerResponsesSpecial);

            RestartMessages();
            return;
        }

        Debug.Log("Generating Normal Prayer Set");

        List<string> selectedGoodLine = GetRandomUniqueLines(GoodLines, 1);
        List<string> selectedBadLines = GetRandomUniqueLines(BadLines, 2);

        List<PrayerResponse> prayerResponses = new List<PrayerResponse>();
        GoodIdx = Random.Range(0, 3);
        int badCount = 0;
        List<int> selectedSpecials = new List<int>();

        _SpecialPrayerActive = false;
        for (int i = 0; i < 3; i++)
        {
            string[] split;
            if (GoodIdx == i)
            {
                split = selectedGoodLine[0].Split(" @");
                PrayerResponse newGoodResponse = new PrayerResponse
                {
                    AssociatedIdx = -1,
                    Prayer = split[0],
                    Author = split[1],
                    GoodPrayer = true,
                    SpecialPrayer = false
                };

                prayerResponses.Add(newGoodResponse);
                continue;
            }

            int SpecialPrayerCount = SPrayerSubmissionScript.WaitingSpecialPrayers.Count;
            if (SPrayerSubmissionScript.WaitingSpecialPrayers.Count > 0)
            {
                Debug.Log("Testing for special prayers.");
                if (Random.value < 0.2f && !GameStateMonitor.isEventActive())
                {
                    Debug.Log("Adding special prayer to mix.");

                    int selectedPrayerIdx = Random.Range(0, SpecialPrayerCount);
                    SpecialPrayerData selectedPrayer = SPrayerSubmissionScript.WaitingSpecialPrayers[selectedPrayerIdx];

                    if (!selectedSpecials.Contains(selectedPrayerIdx))
                    {
                        _SpecialPrayerActive = true;
                        selectedSpecials.Add(selectedPrayerIdx);
                        PrayerResponse newResponse = new PrayerResponse
                        {
                            AssociatedIdx = -1,
                            Prayer = selectedPrayer.Option,
                            Author = selectedPrayer.AuthorName,
                            GoodPrayer = selectedPrayer.GoodPrayer,
                            SpecialPrayer = true,
                            AssociatedSpecialPrayerData = selectedPrayer
                        };

                        prayerResponses.Add(newResponse);
                        continue;
                    }
                }
            }

            split = selectedBadLines[badCount].Split(" @");
            PrayerResponse newBadResponse = new PrayerResponse
            {
                AssociatedIdx = -1,
                Prayer = split[0],
                Author = split[1],
                GoodPrayer = false,
                SpecialPrayer = false
            };

            prayerResponses.Add(newBadResponse);
            badCount++;
        }
        CreateVariableLengthOptions(prayerResponses);

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
