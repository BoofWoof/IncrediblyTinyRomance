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

    public MoodInterface Mood;

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

    public void ForceAnimation(string stateName)
    {
        int stateHash = Animator.StringToHash(stateName);
        if (!CharacterAnimator.HasState(0, stateHash))
        {
            Debug.LogError("No animation state with that name.");
            return;
        }
        CharacterAnimator.Play(stateName);
    }

    public void ProcessGesture(string GestureName)
    {
        Debug.Log("Running Gesture: " + GestureName);
        string[] GestureData = GestureName.Split(" ");

        switch (GestureData[0])
        {
            case "Sit":
                CharacterAnimator.SetBool("Sitting", true);
                CharacterAnimator.SetBool("Looming", false);
                break;
            case "Stand":
                CharacterAnimator.SetBool("Sitting", false);
                CharacterAnimator.SetBool("Looming", false);
                break;
            case "SitForward":
                CharacterAnimator.SetTrigger("SitForward");
                break;
            case "Loom":
                CharacterAnimator.SetBool("Looming", true);
                break;
            case "LeftGrab":
                CharacterAnimator.SetBool("LeftGrab", true);
                break;
            case "LeftUnGrab":
                CharacterAnimator.SetBool("LeftGrab", false);
                break;
            case "Anger":
                Mood.SetAnger(float.Parse(GestureData[1]));
                break;
        }
    }

    public void ProcessText()
    {
        Func<string, string> StringIdentityProcess = (s) => s;
        TimeMarkers = Gestures.text.ToTimeMarkers<string>(", ", StringIdentityProcess, FPS);
    }
}
