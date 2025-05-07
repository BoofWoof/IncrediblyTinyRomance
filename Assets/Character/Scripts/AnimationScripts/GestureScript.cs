using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureScript : MonoBehaviour
{
    public TextAsset Gestures;
    public float FPS = 24f;

    private TimeList<string> TimeMarkers;

    public GestureListSO ValidGestures;

    Coroutine GestureCoroutine;

    public void PlaySpeech()
    {
        ProcessText();

        if (!(GestureCoroutine is null)) StopCoroutine(GestureCoroutine);
        GestureCoroutine = StartCoroutine(PlayGestures());
    }

    public IEnumerator PlayGestures()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        while (audioSource.isPlaying)
        {
            (TimeMarker<string> currentGesture, _) = TimeMarkers.GetNearestData(audioSource.time);
            if (!ValidGestures.ValidGestures.Contains(currentGesture.data)) Debug.LogError("Gesture not found: " + currentGesture.data);

            yield return null;
        }
    }

    public void ProcessText()
    {
        Func<string, string> StringIdentityProcess = (s) => s;
        TimeMarkers = Gestures.text.ToTimeMarkers<string>(", ", StringIdentityProcess, FPS);
    }
}
