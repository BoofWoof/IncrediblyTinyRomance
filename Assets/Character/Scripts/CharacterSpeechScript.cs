using UnityEngine;

public class CharacterSpeechScript : MonoBehaviour
{
    public CharacterSubtitleScript CharacterSubtitle;
    public LipSyncScript LipSync;
    public GestureScript Gesture;

    public void PlaySpeech()
    {
        LipSync.PlaySpeech();
        CharacterSubtitle.PlaySpeech();
        Gesture.PlaySpeech();
    }
}
