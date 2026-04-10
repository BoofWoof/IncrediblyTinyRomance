using UnityEngine;

public class TrukAppScript : AppScript
{
    public static Canvas PhoneScreenCanvas;
    public Canvas phoneScreenCanvas;

    public int StartSongInt;

    public void StartSong()
    {
        MusicSelectorScript.SetPhoneSong(StartSongInt, true);
    }
    public void EndSong()
    {
        MusicSelectorScript.SetPhoneSong(MusicSelectorScript.instance.DefaultStartSongPhoneID, true);
    }

    private void OnEnable()
    {
        OnShowApp += StartSong;
        OnHideApp += EndSong;
    }

    private void OnDisable()
    {
        OnShowApp -= StartSong;
        OnHideApp -= EndSong;
    }

    new private void Awake()
    {
        base.Awake();
        PhoneScreenCanvas = phoneScreenCanvas;
        Hide(true);
        RegisterInputActions();
    }
}
