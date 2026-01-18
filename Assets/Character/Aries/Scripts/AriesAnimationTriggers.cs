using UnityEngine;

public class AriesAnimationTriggers : MonoBehaviour
{
    public PushupScript PushupEventScript;

    public VoiceLineSO PuffAudioWarningSO;

    public MoodInterface AriesMood;

    public PurificationGameScript PurificationGame;

    public ParticleSystem PuffSystem;
    public void PerformPushup()
    {
        PushupEventScript.Pushup();
    }

    public void SetMood(float newAnger)
    {
        AriesMood.SetAnger(newAnger);
    }

    public void PuffAudioWarning()
    {
        CharacterSpeechScript.BroadcastSpeechAttempt("MacroAries", PuffAudioWarningSO);
    }
    public void StartPuffAttack()
    {
        PurificationGame.StartGame();
    }
    public void Puff()
    {
        PuffSystem.Play();
    }
}
