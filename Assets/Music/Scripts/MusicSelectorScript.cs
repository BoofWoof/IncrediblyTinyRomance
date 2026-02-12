using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct MusicDataStruct
{
    public AudioClip Song;
    public string Name;
    public float MaxVolume;
    public int GroupID;

    public float StartTime;
}

public class MusicSelectorScript : MonoBehaviour
{
    public static MusicSelectorScript instance;

    public int DefaultStartSongOverworldID = 1;
    public static int DefaultOverworldSong {  get; set; }
    public int DefaultStartSongPhoneID = 0;
    public static int DefaultPhoneSong { get; set; }

    public int OverworldSongID;
    public int PhoneMusicID;

    private static bool SongLock = false;

    public MusicDataStruct[] SongList;

    private void OnEnable()
    {
        Lua.RegisterFunction("SetSong", null, SymbolExtensions.GetMethodInfo(() => SetOverworldSong(0)));
        Lua.RegisterFunction("SetPhoneSong", null, SymbolExtensions.GetMethodInfo(() => SetPhoneSong(0)));
        Lua.RegisterFunction("RevertSong", null, SymbolExtensions.GetMethodInfo(() => RevertOverworldSong()));
        Lua.RegisterFunction("RevertPhoneSong", null, SymbolExtensions.GetMethodInfo(() => RevertPhoneSong()));
        PhonePositionScript.PhoneToggled += PhoneToggleMusicSwap;
    }
    private void OnDisable()
    {
        PhonePositionScript.PhoneToggled -= PhoneToggleMusicSwap;
    }

    public static void LockSong()
    {
        SongLock = true;
    }
    public static void ReleaseSong()
    {
        SongLock = false;
    }

    public void Awake()
    {
        instance = this;
        DefaultOverworldSong = DefaultStartSongOverworldID;
        DefaultPhoneSong = DefaultStartSongPhoneID;

        OverworldSongID = DefaultStartSongOverworldID;
        PhoneMusicID = DefaultStartSongPhoneID;
    }

    public static void SetPhoneSong(double newSongID)
    {
        SetPhoneSong(newSongID, false);
    }
    public static void SetPhoneSong(double newSongID, bool instant = false)
    {
        if (SongLock) return;
        instance.PhoneMusicID = (int)newSongID;
        if (PhonePositionScript.raised)
        {
            CrossfadeScript.TransitionSong(instance.PhoneMusicID, instant);
        }
    }
    public static void RevertPhoneSong()
    {
        SetPhoneSong(DefaultPhoneSong);
    }
    public static void SetOverworldSong(double newSongID)
    {
        SetOverworldSong(newSongID, false);
    }
    public static void SetOverworldSong(double newSongID, bool instant = false)
    {
        if (SongLock) return;
        instance.OverworldSongID = (int)newSongID;
        if (!PhonePositionScript.raised)
        {
            CrossfadeScript.TransitionSong(instance.OverworldSongID, instant);
        }
    }
    public static void RevertOverworldSong()
    {
        SetOverworldSong(DefaultOverworldSong);
    }

    public void PhoneToggleMusicSwap(bool raised)
    {
        if (SongLock) return;

        if (raised)
        {
            CrossfadeScript.TransitionSong(PhoneMusicID);
        } else
        {
            CrossfadeScript.TransitionSong(OverworldSongID);
        }
    }
}
