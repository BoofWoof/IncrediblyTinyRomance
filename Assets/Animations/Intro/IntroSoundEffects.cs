using System.Collections;
using UnityEngine;


public static class DayInfo
{
    public static int CurrentDay = 0;
}

public class IntroSoundEffects : MonoBehaviour
{
    public AudioClip Gate;
    public AudioClip Step;

    private AudioSource ThisAudioSource;

    public bool SkipStart = false;

    public int Day = 0;

    public void Awake()
    {
        DayInfo.CurrentDay = Day;
    }

    public void Start()
    {
        ThisAudioSource = GetComponent<AudioSource>();

        //SetupAudioForDay
        MusicSelectorScript.SetOverworldSong(5);
        CrossfadeScript.ResumeMusic();
        CrossfadeScript.SetLowpassOn(true, true);


        if (SkipStart)
        {
            StartDay();
        }
    }

    public void TakeStep()
    {
        ThisAudioSource.clip = Step;
        ThisAudioSource.Play();
    }
    public void OpenGate()
    {
        ThisAudioSource.clip = Gate;
        ThisAudioSource.Play();
    }
    public void StartDay()
    {
        if (Day == 1)
        {
            StartCoroutine(StartDayOne());
        }
        PlayerCam.EnableCameraMovement = true;
        GetComponent<Camera>().enabled = false;
        InputManager.GameStart();
        if (Day == 0)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator StartDayOne()
    {
        QuestManager.SetQuestByIndex(0);
        MessageQueue.addDialogue("Day1Intro");
        CharacterSpeechScript.BroadcastForceGesture("MacroAries", "BannEnterPuff");

        yield return new WaitForSeconds(0.1f);

        OverworldPositionScript.GoTo("A", 6);

        Destroy(gameObject);
    }
}
