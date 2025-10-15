using PixelCrushers.DialogueSystem;
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
    public int DefaultStartSongPhoneID = 0;

    public int OverworldSongID;
    public int PhoneMusicID;

    public MusicDataStruct[] SongList;

    private void OnEnable()
    {
        Lua.RegisterFunction("SwitchSong", null, SymbolExtensions.GetMethodInfo(() => SetOverworldSong(0)));
        PhonePositionScript.PhoneToggled += PhoneToggleMusicSwap;
    }
    private void OnDisable()
    {
        PhonePositionScript.PhoneToggled -= PhoneToggleMusicSwap;
    }

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        OverworldSongID = DefaultStartSongOverworldID;
        PhoneMusicID = DefaultStartSongPhoneID;
        
        CrossfadeScript.InstantStartSong(OverworldSongID);
    }

    public static void SetPhoneSong(double newSongID)
    {
        instance.PhoneMusicID = (int)newSongID;
        if (PhonePositionScript.raised)
        {
            CrossfadeScript.TransitionSong(MusicSelectorScript.instance.PhoneMusicID);
        }
    }
    public static void SetOverworldSong(double newSongID)
    {
        instance.PhoneMusicID = (int)newSongID;
        if (!PhonePositionScript.raised)
        {
            CrossfadeScript.TransitionSong(MusicSelectorScript.instance.OverworldSongID);
        }
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
