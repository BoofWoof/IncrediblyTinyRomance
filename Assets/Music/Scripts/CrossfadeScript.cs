using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossfadeScript : MonoBehaviour
{
    public AudioSource currentTrack;   // AudioSource for the current track
    public float currentVolume;
    public AudioSource newTrack;       // AudioSource for the new track
    public float newVolume;
    public float fadeDuration = 3.0f;  // Duration of the crossfade

    // Start playing the new track with crossfade

    public void Start()
    {
        PhonePositionScript.PhoneToggled += StartCrossFadeTracks;
        currentTrack.volume = currentVolume;
        newTrack.volume = 0;

    }
    private void OnDestroy()
    {
        PhonePositionScript.PhoneToggled -= StartCrossFadeTracks;
    }

    private void StartCrossFadeTracks(bool raised)
    {
        StartCoroutine(CrossfadeTracks());
    }

    private IEnumerator CrossfadeTracks()
    {
        float timeElapsed = 0f;

        float currentTrackStartVolume = currentTrack.volume;
        float newTrackStartVolume = newTrack.volume;

        // Gradually crossfade over the fadeDuration
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / fadeDuration;

            currentTrack.volume = Mathf.Lerp(currentTrackStartVolume, 0f, progress);  // Fade out current track
            newTrack.volume = Mathf.Lerp(newTrackStartVolume, newVolume, progress);           // Fade in new track

            yield return null;
        }

        // Ensure volumes are fully transitioned
        currentTrack.volume = 0f;
        newTrack.volume = newVolume;

        AudioSource tempSource = currentTrack;
        currentTrack = newTrack;
        newTrack = tempSource;

        float tempVolume = currentVolume;
        currentVolume = newVolume;
        newVolume = currentVolume;
    }
}
