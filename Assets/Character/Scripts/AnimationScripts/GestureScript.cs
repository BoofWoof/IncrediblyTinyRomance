using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureScript : MonoBehaviour
{
    public TextAsset Gestures;
    public float FPS = 24f;

    private TimeList<string> TimeMarkers;

    public Animator CharacterAnimator;

    private TimeMarker<string> lastTimeMarker;

    Coroutine GestureCoroutine;

    public void PlaySpeech()
    {
        ProcessText();

        if (!(GestureCoroutine is null)) StopCoroutine(GestureCoroutine);
        GestureCoroutine = StartCoroutine(PlayGestures());
    }

    public IEnumerator PlayGestures()
    {
        lastTimeMarker = new TimeMarker<string>();
        lastTimeMarker.timeSec = -1f;
        AudioSource audioSource = GetComponent<AudioSource>();
        while (audioSource.isPlaying)
        {
            (TimeMarker<string> currentGesture, _) = TimeMarkers.GetNearestData(audioSource.time);
            //if (!ValidGestures.ValidGestures.Contains(currentGesture.data)) Debug.LogError("Gesture not found: " + currentGesture.data);
            if(currentGesture.timeSec != lastTimeMarker.timeSec)
            {
                ProcessGesture(currentGesture.data);

                lastTimeMarker = currentGesture;
            }

            yield return null;
        }
    }

    private void ProcessGesture(string GestureName)
    {
        Debug.Log("Running Gesture: " + GestureName);
        switch (GestureName)
        {
            case "Sit":
                CharacterAnimator.SetBool("Sitting", true);
                break;
            case "Stand":
                CharacterAnimator.SetBool("Sitting", false);
                break;
            case "SitForward":
                CharacterAnimator.SetTrigger("SitForward");
                break;
        }
    }

    public void ProcessText()
    {
        Func<string, string> StringIdentityProcess = (s) => s;
        TimeMarkers = Gestures.text.ToTimeMarkers<string>(", ", StringIdentityProcess, FPS);
    }
}
