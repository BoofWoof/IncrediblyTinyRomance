using UnityEngine;

public class CharacterSpeechScript : MonoBehaviour
{
    public string CharacterName;
    public bool MacroSpeech = false;
    public bool RadioSpeech = false;

    public CharacterSubtitleScript CharacterSubtitle;
    public LipSyncScript LipSync;
    public GestureScript Gesture;

    public VoiceLineSO debugVoiceLine;

    public void PlayDebugVoiceLine()
    {
        PlaySpeech(debugVoiceLine);
    }

    public void PlaySpeech(VoiceLineSO voiceLine)
    {
        if (voiceLine.AudioData != null)
        {
            GetComponent<AudioSource>().clip = voiceLine.AudioData;
        }
        GetComponent<AudioSource>().Play();
        if (voiceLine.PhenomeData != null)
        {
            LipSync.PhenomeAsset = voiceLine.PhenomeData;
            LipSync.PlaySpeech();
        }

        if (voiceLine.SubtitleData != null)
        {
            CharacterSubtitle.Subtitles = voiceLine.SubtitleData;
            CharacterSubtitle.PlaySpeech();
        }
        //THIS NEEDS TO ACTUALLY GET FILLED OUT
        if (voiceLine.GestureData != null)
        {
            Gesture.Gestures = voiceLine.GestureData;
            Gesture.PlaySpeech();
        }
    }

    public bool isSpeechPlaying()
    {
        return GetComponent<AudioSource>().isPlaying;
    }
}
