using UnityEngine;

public class TrukAppScript : AppScript
{
    public static Canvas PhoneScreenCanvas;
    public Canvas phoneScreenCanvas;

    public void StartSong()
    {
        MusicSelectorScript.SetPhoneSong(6);
    }
    public void EndSong()
    {
        MusicSelectorScript.SetPhoneSong(MusicSelectorScript.instance.DefaultStartSongPhoneID);
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

    private void Awake()
    {
        PhoneScreenCanvas = phoneScreenCanvas;
        Hide(true);
        RegisterInputActions();
    }
}
