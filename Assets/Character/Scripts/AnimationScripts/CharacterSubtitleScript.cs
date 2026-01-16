using System;
using System.Collections;
using UnityEngine;

public class CharacterSubtitleScript : MonoBehaviour
{
    public TextAsset Subtitles;
    public float FPS = 24f;

    private TimeList<string> timeMarkers;

    Coroutine SubtitleCoroutine;

    public CharacterSpeechScript speechScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    public void PlaySpeech()
    {
        ProcessText();

        if (!(SubtitleCoroutine is null)) StopCoroutine(SubtitleCoroutine);
        SubtitleCoroutine = StartCoroutine(PlaySubtitles());

    }

    public IEnumerator PlaySubtitles()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        while (speechScript.isSpeechPlaying())
        {
            if(timeMarkers.GetSize() > 0)
            {
                (TimeMarker<string> currentSubtitle, _) = timeMarkers.GetNearestData(audioSource.time);
                SubtitleScript.instance.SetText(currentSubtitle.data);
            } else
            {
                SubtitleScript.instance.SetText("");
            }
            yield return null;
        }
        SubtitleScript.instance.SetText("");
    }

    public void ProcessText()
    {
        Func<string, string> StringIdentityProcess = (s) => s;
        timeMarkers = Subtitles.text.ToTimeMarkers<string>(", " , StringIdentityProcess, FPS);
    }
}
