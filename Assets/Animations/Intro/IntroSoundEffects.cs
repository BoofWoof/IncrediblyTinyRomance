using UnityEngine;

public class IntroSoundEffects : MonoBehaviour
{
    public AudioClip Gate;
    public AudioClip Step;

    private AudioSource ThisAudioSource;

    public bool SkipStart = false;

    public void Start()
    {
        ThisAudioSource = GetComponent<AudioSource>();
        
        if(SkipStart)
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
        StartDayOne();
        PlayerCam.EnableCameraMovement = true;
        GetComponent<Camera>().enabled = false;
        InputManager.GameStart();
        Destroy(gameObject);
    }

    public void StartDayOne()
    {
        QuestManager.SetQuestByIndex(0);
        MessageQueue.addDialogue("Day1Intro");
        CharacterSpeechScript.BroadcastForceGesture("MacroAries", "BannEnterPuff");
    }
}
