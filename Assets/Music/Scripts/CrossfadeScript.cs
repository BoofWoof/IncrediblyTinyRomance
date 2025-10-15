using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CrossfadeScript : MonoBehaviour
{
    public static CrossfadeScript MusicPlayer;

    public AudioSource currentTrack;   // AudioSource for the current track
    public AudioSource oldTrack;       // AudioSource for the new track
    public float fadeDuration = 3.0f;  // Duration of the crossfade

    public MusicDataStruct CurrentSong;
    public Coroutine TransitionCoroutine;

    private static bool MusicPaused = true;

    // Start playing the new track with crossfade


    public void Awake()
    {
        MusicPlayer = this;
    }

    public void Start()
    {
        PauseMusic();

        oldTrack.volume = 0;
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

    public static void InstantStartSong(int SongID)
    {
        if (MusicSelectorScript.instance.SongList[SongID].Name == MusicPlayer.CurrentSong.Name) return;

        MusicPlayer.CurrentSong = MusicSelectorScript.instance.SongList[SongID];
        MusicPlayer.currentTrack.clip = MusicPlayer.CurrentSong.Song;
        MusicPlayer.currentTrack.volume = MusicPlayer.CurrentSong.MaxVolume;

        if(!MusicPaused) MusicPlayer.currentTrack.Play();

        MusicPlayer.currentTrack.time = MusicPlayer.CurrentSong.StartTime;
    }

    public static void TransitionSong(int SongID)
    {
        if (MusicSelectorScript.instance.SongList[SongID].Name == MusicPlayer.CurrentSong.Name) return;

        MusicDataStruct NewSong = MusicSelectorScript.instance.SongList[SongID];

        if (MusicPlayer.CurrentSong.Song != null) {
            MusicSelectorScript.instance.SongList[SongID].StartTime = MusicPlayer.currentTrack.time;
        }

        if (NewSong.GroupID != MusicPlayer.CurrentSong.GroupID)
        {
            InstantStartSong(SongID);
            return;
        }

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
