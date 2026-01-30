using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

struct WaitingExpansion
{
    public BADdirections expansionDirection;
    public PipeStackScript sourceVent;
}
struct VentRouteData
{
    public PipeStackScript SourceVent;
    public List<PipeStackScript> PrimaryExpanded;
    public List<PipeStackScript> SecondaryExpanded;
    public List<WaitingExpansion> WaitingExpansions;
    public bool LeakFound;
    public List<PipeStackScript> GoalsFound;
    public bool GoalFound;
}
public class PurificationGameScript : MonoBehaviour
{
    public static PurificationGameScript instance;
    public static PurificationHolderScript associatedLevelHolder;

    public PurificationLevelPacksSO CurrentLevelPack;
    public int CurrentLevelInPack = 0;

    public VentGridScript VentGridData;
    private List<WaitingExpansion> PrevDeadEndExpansions;

    public GameObject MainBackdrop;
    public GameObject WinText;

    public ParticleSystem FogSource;

    public AudioSource VentRotationAS;
    public AudioSource WinAS;

    public Volume PuffVolume;

    public float StartingTimerTime = 60f;
    public float TimerTime;
    public TMP_Text TimerText;
    public bool TimerEnabled = true;
    public bool LevelRunning = false;

    public static float TotalTime = 0;
    public float StartingTime;

    public void Start()
    {
        instance = this;
        PipeStackScript.VentRotationEvent += UpdatePipeRoutes;
        PipeStackScript.VentRotationStartEvent += VentRotationAS.Play;

        PuffVolume.weight = 0f;
    }

    public static void SetLevelSet(PurificationLevelPacksSO newLevelPack)
    {
        instance.CurrentLevelPack = newLevelPack;
    }

    public void StartGame()
    {
        ChannelChanger.instance.StartCoroutine(VolumeFade(1f));

        if (ChannelChanger.DangerActive) return;
        ChannelChanger.ActiveChannelChanger.PuritySwitch();
        ChannelChanger.DangerActive = true;

        StartingTime = Time.time;

        CurrentLevelInPack = 0;
        MusicSelectorScript.SetOverworldSong(5);
        FogSource.Play();
        LevelRunning = true;
        StartLevel();
    }

    public void StartLevel()
    {
        if(CurrentLevelPack.Levels[CurrentLevelInPack].HallucinationBroadcasts.Count > 0)
        {
            PlayerBlinkScript.StartBlink(CurrentLevelPack.Levels[CurrentLevelInPack].HallucinationBroadcasts);
        }

        if (CurrentLevelPack.Levels[CurrentLevelInPack].VoiceLine != null)
        {
            CharacterSpeechScript.BroadcastSpeechAttempt(CurrentLevelPack.Levels[CurrentLevelInPack].VoiceLineTargetName, CurrentLevelPack.Levels[CurrentLevelInPack].VoiceLine);
        }


        PurificationLevelSO LevelData = CurrentLevelPack.Levels[CurrentLevelInPack];
        TimerEnabled = LevelData.EnableTimer;
        TimerText.gameObject.SetActive(TimerEnabled);
        StartingTimerTime = LevelData.PuzzleTimeLimit;
        TimerTime = StartingTimerTime;

        VentGridData.SpawnGridFromSaveData(LevelData);
        UpdatePipeRoutes();
        PipeStackScript.GlobalRotationAllowed = true;
    }

    public void Update()
    {
        if (!LevelRunning) return;
        if (!TimerEnabled) return;

        TimerTime -= Time.deltaTime;
        if(TimerTime <= 0)
        {
            AnnouncementScript.StartAnnouncement("The air grows thicker. Your people are losing their grip on reality.");
            DefenseStats.DamageCity(10f);
            TimerTime = StartingTimerTime;
        }
        TimerText.text = "Oversaturation In: <b>" + System.TimeSpan.FromSeconds(TimerTime).ToString("m\\:ss") + "</b>";
    }

    public void UpdatePipeRoutes()
    {
        ResetPipeRoutes();

        List<VentRouteData> ventRoutes = new List<VentRouteData>();
        List<WaitingExpansion> DeadEndExpansions = new List<WaitingExpansion>();

        bool GoalsMissing = false;
        bool DeadPipesFound = false;

        //Find the routes for every source.
        foreach (PipeStackScript fumeSource in VentGridData.Sources)
        {
            PipeStackScript currentVent = fumeSource;

            //Find valid expansion directions.
            List<BADdirections> possibleExpansionDirection = currentVent.GetPossibleExpansions(BADdirections.NULL);

            foreach (BADdirections expansionDirection in possibleExpansionDirection)
            {
                VentRouteData newVentRouteData = new VentRouteData();

                newVentRouteData.SourceVent = currentVent;
                currentVent.SetLightActive();

                newVentRouteData.PrimaryExpanded = new List<PipeStackScript>();
                newVentRouteData.SecondaryExpanded = new List<PipeStackScript>();
                newVentRouteData.WaitingExpansions = new List<WaitingExpansion>();
                newVentRouteData.GoalFound = false;
                newVentRouteData.LeakFound = false;
                newVentRouteData.GoalsFound = new List<PipeStackScript>();

                newVentRouteData.PrimaryExpanded.Add(currentVent);

                WaitingExpansion newWaitingExpansion = new WaitingExpansion();
                newWaitingExpansion.expansionDirection = expansionDirection;
                newWaitingExpansion.sourceVent = currentVent;
                newVentRouteData.WaitingExpansions.Add(newWaitingExpansion);

                Debug.Log("----");

                int i = 0; //I is just being used to avoid infinite loops while debugging.
                while (newVentRouteData.WaitingExpansions.Count > 0)
                {
                    //Pop closest expansion:
                    WaitingExpansion expansionData = newVentRouteData.WaitingExpansions[0];
                    newVentRouteData.WaitingExpansions.RemoveAt(0);

                    expansionData.sourceVent.SetLightActive();

                    (bool validVent, bool secondaryVent, PipeStackScript nextVent) = VentGridData.ExpansionCheck(expansionData.expansionDirection, expansionData.sourceVent.VentPosID);


                    if (!validVent)
                    {
                        newVentRouteData.LeakFound = true;
                        DeadEndExpansions.Add(expansionData);
                        continue;
                    }

                    if (nextVent.isGoal)
                    {
                        newVentRouteData.GoalFound = true;
                        newVentRouteData.GoalsFound.Add(nextVent);
                        continue;
                    }

                    if (nextVent.isSource) continue;

                    Debug.DrawLine(expansionData.sourceVent.transform.position, nextVent.transform.position, Color.red, 30f);

                    if (secondaryVent)
                    {
                        if (newVentRouteData.SecondaryExpanded.Contains(nextVent)) continue;
                        newVentRouteData.SecondaryExpanded.Add(nextVent);
                    } else
                    {
                        if (newVentRouteData.PrimaryExpanded.Contains(nextVent)) continue;
                        newVentRouteData.PrimaryExpanded.Add(nextVent);
                    }

                    possibleExpansionDirection = nextVent.GetPossibleExpansions(expansionData.expansionDirection);

                    foreach (BADdirections expansionDirection2 in possibleExpansionDirection)
                    {
                        //Debug.Log(expansionDirection2);
                        WaitingExpansion newWaitingExpansion2 = new WaitingExpansion();
                        newWaitingExpansion2.expansionDirection = expansionDirection2;
                        newWaitingExpansion2.sourceVent = nextVent;
                        newVentRouteData.WaitingExpansions.Add(newWaitingExpansion2);
                    }

                    //Delete this later, just to make sure I don't lock my computer.
                    i++;
                    if (i > 1000) break;
                }

                if (!newVentRouteData.LeakFound)
                {
                    foreach(PipeStackScript goalVent in newVentRouteData.GoalsFound)
                    {
                        goalVent.SetLightActive();
                    }
                }

                foreach (WaitingExpansion deadExpansion in DeadEndExpansions)
                {
                    deadExpansion.sourceVent.SetLightLeaking();
                    DeadPipesFound = true;
                }
                if(!newVentRouteData.GoalFound) GoalsMissing = true;

                ventRoutes.Add(newVentRouteData);
            }
        }
        foreach (VentRouteData ventRoute in ventRoutes)
        {
            Debug.Log(ventRoute.GoalFound);
            if (!ventRoute.GoalFound) ventRoute.SourceVent.SetLightLeaking();
        }

        if(!GoalsMissing && !DeadPipesFound)
        {
            Win();
        }
        if(PrevDeadEndExpansions != null)
        {
            foreach (WaitingExpansion prevDeadEnd in PrevDeadEndExpansions)
            {
                bool stillDeadEnd = false;
                foreach (WaitingExpansion newDeadEnd in DeadEndExpansions)
                {
                    if(prevDeadEnd.sourceVent == newDeadEnd.sourceVent && prevDeadEnd.expansionDirection == newDeadEnd.expansionDirection)
                    {
                        stillDeadEnd = true;
                        break;
                    }
                }
                if (!stillDeadEnd)
                {
                    int adjacentVentID = VentGridData.ConvertVector2ToPosID(prevDeadEnd.sourceVent.VentPosID, prevDeadEnd.expansionDirection);
                    if (adjacentVentID > 0)
                    {
                        PipeStackScript adjacentVentScript = VentGridData.PipeStacks[adjacentVentID].GetComponent<PipeStackScript>();
                        switch (prevDeadEnd.expansionDirection)
                        {
                            case BADdirections.UP:
                                adjacentVentScript.DownLeakParticles.SetActive(false);
                                break;
                            case BADdirections.RIGHT:
                                adjacentVentScript.LeftLeakParticles.SetActive(false);
                                break;
                            case BADdirections.DOWN:
                                adjacentVentScript.UpLeakParticles.SetActive(false);
                                break;
                            case BADdirections.LEFT:
                                adjacentVentScript.RightLeakParticles.SetActive(false);
                                break;
                        }
                    }
                }
            }
        }
        PrevDeadEndExpansions = DeadEndExpansions;
    }

    public void Win()
    {
        string cutsceneName = CurrentLevelPack.Levels[CurrentLevelInPack].ConnectedCutsceneName;
        if (cutsceneName.Length > 0) MessageQueue.addDialogue(CurrentLevelPack.Levels[CurrentLevelInPack].ConnectedCutsceneName);

        Debug.Log("Purification: YOUWIN");
        PipeStackScript.GlobalRotationAllowed = false;
        SpawnWinScreen();
        WinAS.Play();
    }

    public void ResetPipeRoutes()
    {
        foreach (GameObject vent in VentGridData.PipeStacks)
        {
            vent.GetComponent<PipeStackScript>().SetLightIdle();
        }
    }

    public void NextLevel()
    {
        CurrentLevelInPack += 1;
        if(CurrentLevelInPack < CurrentLevelPack.Levels.Count)
        {
            Debug.Log("LoadingNextLevel");
            StartLevel();
        }
        else
        {
            TotalTime += Time.time - StartingTime;

            ChannelChanger.instance.StartCoroutine(VolumeFade(0f));

            Debug.Log("LevelPackComplete");
            VentGridData.ClearGrid();
            FogSource.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            MusicSelectorScript.SetOverworldSong(1);

            ChannelChanger.DangerActive = false;
            ChannelChanger.ActiveChannelChanger.LockSwitch();

            if (associatedLevelHolder.HallucinationResets.Count > 0)
            {
                PlayerBlinkScript.StartBlink(associatedLevelHolder.HallucinationResets);
            }

            LevelRunning = false;

            OverworldBehavior.AriesBehavior("judge");
        }
    }

    public void SpawnWinScreen()
    {
        GameObject newWinText = Instantiate(WinText);
        newWinText.transform.parent = MainBackdrop.transform;
        newWinText.transform.localScale = Vector3.one;
        newWinText.transform.rotation = Quaternion.identity;
        newWinText.transform.localPosition = Vector3.zero;

        newWinText.GetComponent<MaterialGradient>().OnCompletion.AddListener(NextLevel);
    }

    public IEnumerator VolumeFade(float finalValue)
    {
        float timePassed = 0f;
        float transitionPeriod = 4f;

        float startingValue = PuffVolume.weight;

        while (timePassed < transitionPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / transitionPeriod;

            PuffVolume.weight = Mathf.Lerp(startingValue, finalValue, progress);

            yield return null;
        }

        PuffVolume.weight = finalValue;
    }
}
