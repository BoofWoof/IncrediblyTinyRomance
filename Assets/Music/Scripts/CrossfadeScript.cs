using PixelCrushers.DialogueSystem.Articy.Articy_4_0;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class CrossfadeScript : MonoBehaviour
{
    public static CrossfadeScript MusicPlayer;

    public AudioSource currentTrack;   // AudioSource for the current track
    public AudioSource oldTrack;       // AudioSource for the new track
    public float fadeDuration = 3.0f;  // Duration of the crossfade

    public int CurrentSongID;
    public MusicDataStruct CurrentSong;
    public Coroutine TransitionCoroutine;
    public Coroutine CutoffTransitionCoroutine;

    private static bool MusicPaused = true;

    public AudioMixer masterMixer;
    public bool cutoffTriggered = false;
    public float lowpassMinCutoff = 321;
    public float lowpassMaxCutoff = 5000;
    public float transitionPeriod = 1f;
    Coroutine lowpassTransition;

    // Start playing the new track with crossfade


    public void Awake()
    {
        MusicPlayer = this;
        PauseMusic();
        oldTrack.volume = 0;
    }

    public static void SetLowpassOn(bool LowpassOn, bool instant = false)
    {
        if (LowpassOn == MusicPlayer.cutoffTriggered) return;
        MusicPlayer.cutoffTriggered = LowpassOn;
        float newCutoff;
        if (LowpassOn)
        {
            newCutoff = MusicPlayer.lowpassMinCutoff;
        } else
        {
            newCutoff = MusicPlayer.lowpassMaxCutoff;
        }
        if (instant)
        {
            InstantCutoff(newCutoff);
            return;
        }
        if(MusicPlayer.CutoffTransitionCoroutine != null) MusicPlayer.StopCoroutine(MusicPlayer.CutoffTransitionCoroutine);
        MusicPlayer.CutoffTransitionCoroutine = MusicPlayer.StartCoroutine(MusicPlayer.cutoffTransition(newCutoff));
    }

    public static void InstantCutoff(float cutoff)
    {
        MusicPlayer.masterMixer.SetFloat("MusicLowpass", cutoff);
    }

    public IEnumerator cutoffTransition(float newCutoff)
    {
        float timePassed = 0f;
        float startingValue;
        MusicPlayer.masterMixer.GetFloat("MusicLowpass", out startingValue);
        while (timePassed < transitionPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed/ transitionPeriod;

            MusicPlayer.masterMixer.SetFloat("MusicLowpass", Mathf.Lerp(startingValue, newCutoff, progress));

            yield return null;
        }
        MusicPlayer.masterMixer.SetFloat("MusicLowpass", newCutoff);
    }

    public static void PauseMusic()
    {
        MusicPlayer.currentTrack.Pause();
        MusicPlayer.oldTrack.Pause();

        MusicPaused = true;
    }

    public static void ResumeMusic()
    {
        MusicPlayer.currentTrack.Play();
        MusicPlayer.oldTrack.Play();

        MusicPaused = false;
    }

    public static void InstantStartSong(int SongID, bool TrulyInstant = false)
    {
        if (MusicSelectorScript.instance.SongList[SongID].Name == MusicPlayer.CurrentSong.Name) return;

        if (MusicPlayer.TransitionCoroutine != null) MusicPlayer.StopCoroutine(MusicPlayer.TransitionCoroutine);

        MusicPlayer.CurrentSongID = SongID;
        MusicPlayer.CurrentSong = MusicSelectorScript.instance.SongList[SongID];
        if (TrulyInstant)
        {
            MusicPlayer.oldTrack.volume = 0;

            MusicPlayer.currentTrack.clip = MusicPlayer.CurrentSong.Song;
            MusicPlayer.currentTrack.volume = MusicPlayer.CurrentSong.MaxVolume;

            if (!MusicPaused) MusicPlayer.currentTrack.Play();

            MusicPlayer.currentTrack.time = MusicPlayer.CurrentSong.StartTime;
            return;
        }
        Debug.Log("BBBBBBBBB");
        MusicPlayer.TransitionCoroutine = MusicPlayer.StartCoroutine(MusicPlayer.FadeInAndOutTransition(MusicSelectorScript.instance.SongList[SongID]));
    }

    public IEnumerator FadeInAndOutTransition(MusicDataStruct NewSong)
    {
        float timeElapsed = 0f;

        AudioSource tempSource = MusicPlayer.oldTrack;
        MusicPlayer.oldTrack = MusicPlayer.currentTrack;
        MusicPlayer.currentTrack = tempSource;
        MusicPlayer.currentTrack.clip = NewSong.Song;

        float currentTrackStartVolume = MusicPlayer.currentTrack.volume;
        float oldTrackStartVolume = MusicPlayer.oldTrack.volume;

        // Gradually crossfade over the fadeDuration
        while (timeElapsed < MusicPlayer.fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / MusicPlayer.fadeDuration;

            MusicPlayer.oldTrack.volume = Mathf.Lerp(oldTrackStartVolume, 0, progress);

            yield return null;
        }
        MusicPlayer.oldTrack.volume = 0f;
        MusicPlayer.oldTrack.Stop();

        yield return new WaitForSeconds(1f);

        MusicPlayer.currentTrack.Play();
        timeElapsed = 0f;
        // Gradually crossfade over the fadeDuration
        while (timeElapsed < MusicPlayer.fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / MusicPlayer.fadeDuration;

            MusicPlayer.currentTrack.volume = Mathf.Lerp(currentTrackStartVolume, NewSong.MaxVolume, progress);

            yield return null;
        }
        MusicPlayer.currentTrack.volume = NewSong.MaxVolume;
    }

    public static void TransitionSong(int SongID, bool instant = false)
    {
        if (MusicSelectorScript.instance.SongList[SongID].Name == MusicPlayer.CurrentSong.Name) return;

        MusicDataStruct NewSong = MusicSelectorScript.instance.SongList[SongID];

        if (MusicPlayer.CurrentSong.Song != null) {
            MusicSelectorScript.instance.SongList[MusicPlayer.CurrentSongID].StartTime = MusicPlayer.currentTrack.time;
        }

        if (NewSong.GroupID != MusicPlayer.CurrentSong.GroupID)
        {
            InstantStartSong(SongID, instant);
            return;
        }

        MusicPlayer.CurrentSongID = SongID;
        MusicPlayer.CurrentSong = MusicSelectorScript.instance.SongList[SongID];

        if (MusicPlayer.TransitionCoroutine != null) MusicPlayer.StopCoroutine(MusicPlayer.TransitionCoroutine);
        MusicPlayer.TransitionCoroutine = MusicPlayer.StartCoroutine(CrossfadeTracks(NewSong));
    }

    private static IEnumerator CrossfadeTracks(MusicDataStruct NewSong)
    {
        float timeElapsed = 0f;

        AudioSource tempSource = MusicPlayer.oldTrack;
        MusicPlayer.oldTrack = MusicPlayer.currentTrack;
        MusicPlayer.currentTrack = tempSource;
        MusicPlayer.currentTrack.clip = NewSong.Song;

        MusicPlayer.currentTrack.Play();
        MusicPlayer.currentTrack.time = MusicPlayer.oldTrack.time;

        float currentTrackStartVolume = MusicPlayer.currentTrack.volume;
        float oldTrackStartVolume = MusicPlayer.oldTrack.volume;

        // Gradually crossfade over the fadeDuration
        while (timeElapsed < MusicPlayer.fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / MusicPlayer.fadeDuration;

            MusicPlayer.currentTrack.volume = Mathf.Lerp(currentTrackStartVolume, NewSong.MaxVolume, progress);  // Fade out current track
            MusicPlayer.oldTrack.volume = Mathf.Lerp(oldTrackStartVolume, 0, progress);           // Fade in new track

            yield return null;
        }

        // Ensure volumes are fully transitioned
        MusicPlayer.oldTrack.volume = 0f;
        MusicPlayer.currentTrack.volume = NewSong.MaxVolume;
    }
}
