using UnityEngine;

[CreateAssetMenu(fileName = "VoiceLineSO", menuName = "VoiceLineSO")]
public class VoiceLineSO : ScriptableObject
{
    public AudioClip AudioData;
    public TextAsset PhenomeData;
    public TextAsset SubtitleData;
    public TextAsset GestureData;
    public float FocusLevel = 1;
}
