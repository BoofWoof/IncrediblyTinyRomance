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

    public void Awake()
    {
        instance = this;
        DefaultOverworldSong = DefaultStartSongOverworldID;
        DefaultPhoneSong = DefaultStartSongPhoneID;

        OverworldSongID = DefaultStartSongOverworldID;
        PhoneMusicID = DefaultStartSongPhoneID;
    }

    public void Start()
    {
    }

    public static void SetPhoneSong(double newSongID)
    {
        instance.PhoneMusicID = (int)newSongID;
        if (PhonePositionScript.raised)
        {
            CrossfadeScript.TransitionSong(instance.PhoneMusicID);
        }
    }
    public static void RevertPhoneSong()
    {
        SetPhoneSong(DefaultPhoneSong);
    }
    public static void SetOverworldSong(double newSongID)
    {
        instance.OverworldSongID = (int)newSongID;
        if (!PhonePositionScript.raised)
        {
            CrossfadeScript.TransitionSong(instance.OverworldSongID);
        }
    }
    public static void RevertOverworldSong()
    {
        SetOverworldSong(DefaultOverworldSong);
    }

    public void PhoneToggleMusicSwap(bool raised)
    {
        if (raised)
        {
            CrossfadeScript.TransitionSong(PhoneMusicID);
        } else
        {
            CrossfadeScript.TransitionSong(OverworldSongID);
        }
    }
}
