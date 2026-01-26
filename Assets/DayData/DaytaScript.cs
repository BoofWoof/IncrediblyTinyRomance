using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class DayInfo
{
    public static int CurrentDay = 0;
}

public class DaytaScript : MonoBehaviour
{
    public static bool SkipStart = false;
    public bool SkipStartInit = false;

    public int DayInit = 0;

    public AudioSource AudioBoom;

    public Image TitleCard;
    public TMP_Text TitleText;

    public void Awake()
    {
        DayInfo.CurrentDay = DayInit;
        SkipStart = SkipStartInit;
    }

    public void Start()
    {
        //SetupAudioForDay
        MusicSelectorScript.SetOverworldSong(5);
        CrossfadeScript.ResumeMusic();
        CrossfadeScript.SetLowpassOn(true, true);


        if (SkipStart)
        {
            StartDay();
        }
    }
    public void StartDay()
    {
        if (DayInfo.CurrentDay == 0)
        {
            EnableCharacter();
        }
        if (DayInfo.CurrentDay == 1)
        {
            StartCoroutine(StartDayOne());
        }
    }

    public IEnumerator StartDayOne()
    {
        QuestManager.SetQuestByIndex(0);
        CharacterSpeechScript.BroadcastForceGesture("MacroAries", "BannEnterPuff");

        yield return new WaitForSeconds(0.1f);

        OverworldPositionScript.GoTo("A", 6);

        TitleCard.gameObject.SetActive(true);
        AudioBoom.Play();

        yield return new WaitForSeconds(2);

        TitleText.gameObject.SetActive(true);
        AudioBoom.Play();

        yield return new WaitForSeconds(3);

        Destroy(TitleText.gameObject);
        Destroy(TitleCard.gameObject);

        MessageQueue.addDialogue("Day1Intro");

        EnableCharacter();
    }

    public void EnableCharacter()
    {
        PlayerCam.EnableCameraMovement = true;
        InputManager.GameStart();
    }
}
