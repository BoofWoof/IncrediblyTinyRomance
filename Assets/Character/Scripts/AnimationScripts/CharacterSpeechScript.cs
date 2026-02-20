using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.IO;

public class CharacterSpeechScript : MonoBehaviour
{
    public bool IsCentralNode = false;
    public static CharacterSpeechScript CentralNode;

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
        if (IsCentralNode) CentralNode = this;
        else CharacterSpeechInstances.Add(this);

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

    public static void BroadcastForceGesture(string name, string gestureName)
    {
        foreach (CharacterSpeechScript characterSpeechScript in CharacterSpeechInstances)
        {
            characterSpeechScript.ForceGesture(name, gestureName);
        }
    }

    public void GestureParameter(string name, string gestureParamter)
    {
        if (SpeakerName.ToLower() != name.ToLower() && NickName.ToLower() != name.ToLower()) return;

        Gesture.ProcessGesture(gestureParamter);
    }

    public static void BroadcastGestureParameter(string name, string gestureParamter)
    {
        foreach (CharacterSpeechScript characterSpeechScript in CharacterSpeechInstances)
        {
            characterSpeechScript.GestureParameter(name, gestureParamter);
        }
    }

    public static void BroadcastShutUp()
    {
        foreach (CharacterSpeechScript characterSpeechScript in CharacterSpeechInstances)
        {
            characterSpeechScript.ShutUp();
        }
    }
    public void ShutUp()
    {
        GetComponent<AudioSource>().Stop();
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

        StartCoroutine(LoadConversationLine(voiceFilePath));
        //VoiceLineSO voiceLine = Resources.Load<VoiceLineSO>(voiceFilePath);
    }

    IEnumerator LoadConversationLine(string voiceFilePath)
    {
        ResourceRequest request = Resources.LoadAsync<VoiceLineSO>(voiceFilePath); // Replace GameObject with your asset type
        
        yield return request;

        VoiceLineSO voiceLine = request.asset as VoiceLineSO;

        if (voiceLine == null) yield break;

        StartCoroutine(Speak(voiceLine));
    }

    public static void BroadcastSpeechAttempt(string name, VoiceLineSO voiceLine)
    {
        foreach (CharacterSpeechScript c in CharacterSpeechInstances)
        {
            if (voiceLine.SpeakerOverride.Length > 0) name = voiceLine.SpeakerOverride;
            c.PlaySpeech(name, voiceLine);
        }
    }
    public static void BroadcastSpeechAttempt(string name, List<VoiceLineSO> voiceLines)
    {
        CentralNode.StartCoroutine(SpeakNoDialogueChain(name, voiceLines));
    }

    public static IEnumerator SpeakNoDialogueChain(string name, List<VoiceLineSO> voiceLines)
    {
        foreach (VoiceLineSO voiceLine in voiceLines)
        {
            foreach (CharacterSpeechScript c in CharacterSpeechInstances)
            {
                string finalName;
                if (voiceLine.SpeakerOverride.Length > 0) finalName = voiceLine.SpeakerOverride;
                else finalName = name;
                if (c.SpeakerName.ToLower() != finalName.ToLower() && c.NickName.ToLower() != finalName.ToLower()) continue;
                yield return c.SpeakNoDialogue(voiceLine);
            }
        }
    }

    public void PlaySpeech(string name, VoiceLineSO voiceLine)
    {
        if (SpeakerName.ToLower() != name.ToLower() && NickName.ToLower() != name.ToLower()) return;
        StartCoroutine(SpeakNoDialogue(voiceLine));
    }
    public IEnumerator SpeakNoDialogue(VoiceLineSO voiceLine)
    {
        GameStateMonitor.AddSpeakingSource(this);

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

        GameStateMonitor.RemoveSpeakingSource(this);
    }
    public IEnumerator Speak(VoiceLineSO voiceLine)
    {
        GameStateMonitor.AddSpeakingSource(this);

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

        GameStateMonitor.RemoveSpeakingSource(this);
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
        if (audioSource.clip == null) Debug.LogError("NO AUDIO FILE ASSIGNED TO DIALOGUE");
        if(!audioSource.isPlaying && (audioSource.timeSamples >= audioSource.clip.samples || audioSource.timeSamples == 0) && MenuTrigger.GetMenuCount() == 0) return false;

        return true;
    }
}
