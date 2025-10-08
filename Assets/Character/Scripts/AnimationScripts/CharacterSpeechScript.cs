using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using NUnit.Framework;
using System.Collections.Generic;

public class CharacterSpeechScript : MonoBehaviour
{
    public string SpeakerName;
    public string NickName;
    public bool MacroSpeech = false;
    public bool RadioSpeech = false;

    public GameObject RadioObject;

    public CharacterSubtitleScript CharacterSubtitle;
    public LipSyncScript LipSync;
    public GestureScript Gesture;

    public VoiceLineSO debugVoiceLine;

    public static List<CharacterSpeechScript> CharacterSpeechInstances = new List<CharacterSpeechScript>();

    private void OnEnable()
    {
        CharacterSpeechInstances.Add(this);

        ConversationManagerScript.instance.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationLine.AddListener(OnConversationLine);
    }

    public void OnDisable()
    {
        CharacterSpeechInstances.Remove(this);

        if (ConversationManagerScript.instance != null)
        {
            ConversationManagerScript.instance.GetComponent<DialogueSystemEvents>().conversationEvents.onConversationLine.RemoveListener(OnConversationLine);
        }
    }

    public void ForceGesture(string name, string gestureName)
    {
        if (SpeakerName.ToLower() != name.ToLower() && NickName.ToLower() != name.ToLower()) return;

        Gesture.ForceAnimation(gestureName);
    }

    public void GestureParameter(string name, string gestureParamter)
    {
        if (SpeakerName.ToLower() != name.ToLower() && NickName.ToLower() != name.ToLower()) return;

        Gesture.ProcessGesture(gestureParamter);
    }

    public void Start()
    {
        if (RadioSpeech)
        {
            RadioObject.SetActive(false);
        }
    }

    public void PlayDebugVoiceLine()
    {
        PlaySpeech(debugVoiceLine);
    }

    public void OnConversationLine(Subtitle subtitle)
    {
        if (!subtitle.speakerInfo.GetFieldBool("IsRadio") && RadioSpeech) return;
        if (!subtitle.speakerInfo.GetFieldBool("IsMacro") && MacroSpeech) return;
        if (subtitle.speakerInfo.Name != SpeakerName) return;

        string voiceFilePath = subtitle.dialogueEntry.fields.Find(f => f.title == "VoiceLinesSO").value;
        voiceFilePath = voiceFilePath.CleanResourcePath();
        VoiceLineSO voiceLine = Resources.Load<VoiceLineSO>(voiceFilePath);

        if (voiceLine == null) return;

        Debug.Log(subtitle.formattedText.text);
        StartCoroutine(Speak(voiceLine));
    }
    public void PlaySpeech(string name, VoiceLineSO voiceLine)
    {
        if (SpeakerName.ToLower() != name.ToLower() && NickName.ToLower() != name.ToLower()) return;

        StartCoroutine(SpeakNoDialogue(voiceLine));
    }
    public IEnumerator SpeakNoDialogue(VoiceLineSO voiceLine)
    {
        //Update play speech to allow animations before and after start.
        yield return new WaitForSeconds(voiceLine.PauseBeforeStart);
        PlaySpeech(voiceLine);
        if (RadioSpeech) RadioObject.SetActive(true);
        yield return null;
        while (isSpeechPlaying())
        {
            yield return null;
        }
        yield return new WaitForSeconds(voiceLine.PauseAfterEnd);
        if (RadioSpeech) RadioObject.SetActive(false);
    }
    public IEnumerator Speak(VoiceLineSO voiceLine)
    {
        //Update play speech to allow animations before and after start.
        yield return new WaitForSeconds(voiceLine.PauseBeforeStart);
        PlaySpeech(voiceLine);
        if (RadioSpeech) RadioObject.SetActive(true);
        yield return null;
        while (isSpeechPlaying())
        {
            yield return null;
        }
        yield return new WaitForSeconds(voiceLine.PauseAfterEnd);
        if (RadioSpeech) RadioObject.SetActive(false);
        (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
    }

    public void PlaySpeech(VoiceLineSO voiceLine)
    {
        if (voiceLine.AudioData != null)
        {
            GetComponent<AudioSource>().clip = voiceLine.AudioData;
        }
        GetComponent<AudioSource>().Play();
        if (voiceLine.PhenomeData != null && LipSync != null)
        {
            LipSync.PhenomeAsset = voiceLine.PhenomeData;
            LipSync.speechScript = this;
            LipSync.PlaySpeech();
        }

        if (voiceLine.SubtitleData != null && CharacterSubtitle != null)
        {
            CharacterSubtitle.Subtitles = voiceLine.SubtitleData;
            CharacterSubtitle.speechScript = this;
            CharacterSubtitle.PlaySpeech();
        }
        //THIS NEEDS TO ACTUALLY GET FILLED OUT
        if (voiceLine.GestureData != null && Gesture != null)
        {
            Gesture.Gestures = voiceLine.GestureData;
            Gesture.speechScript = this;
            Gesture.PlaySpeech();
        }
    }

    public bool isSpeechPlaying()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if(!audioSource.isPlaying && (audioSource.timeSamples >= audioSource.clip.samples || audioSource.timeSamples == 0)) return false;

        return true;
    }
}
