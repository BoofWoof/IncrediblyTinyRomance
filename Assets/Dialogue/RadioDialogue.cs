using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
public class RadioDialogue : MonoBehaviour
{
    //Right now the only speaker is "Radio Milo"
    public readonly string SpeakerName;

    public void OnConversationLine (Subtitle subtitle)
    {
        if(!subtitle.speakerInfo.GetFieldBool("IsRadio")) return;
        if (subtitle.speakerInfo.Name != SpeakerName) return;

        string voiceFilePath = subtitle.dialogueEntry.fields.Find(f => f.title == "VoiceLinesSO").value;
        voiceFilePath = voiceFilePath.CleanResourcePath();
        VoiceLineSO voiceLine = Resources.Load<VoiceLineSO>(voiceFilePath);

        if (voiceLine == null) return;

        Debug.Log(subtitle.formattedText.text);
        StartCoroutine(RadioSpeak(voiceLine));
    }
    public IEnumerator RadioSpeak(VoiceLineSO voiceLine)
    {
        ConversationManagerScript.instance.AriesSpeak(voiceLine);
        yield return null;
        while (ConversationManagerScript.instance.isAriesSpeaking())
        {
            yield return null;
        }
        (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
    }
}
