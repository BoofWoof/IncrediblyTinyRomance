using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenuScript : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMouseSensitivity(float newSensitivity)
    {
        PlayerCam.SetMouseSensitivity(newSensitivity);
    }

    public void SetVibrationIntensity(float newIntensity)
    {
        MoveCamera.SetVibrationIntensity(newIntensity);
    }

    public void SetCursorOpacity(float newCursorOpacity)
    {
        HudScript.SetReticleOpacity(newCursorOpacity);
    }
    public void SetQuestVisiblity(bool questVisibility)
    {
        HudScript.SetQuestVisiblity(questVisibility);
    }
    public void SetSubtitleVisiblity(bool subtitleVisiblity)
    {
        HudScript.SetSubtitleVisiblity(subtitleVisiblity);
    }

    public void SetMasterVolume(float newMasterVolume)
    {
        audioMixer.SetFloat("MasterVolume", newMasterVolume);
    }

    public void SetMusicVolume(float newMusicVolume)
    {
        audioMixer.SetFloat("MusicVolume", newMusicVolume);
    }

    public void SetSFXVolume(float newSFXVolume)
    {
        audioMixer.SetFloat("SFXVolume", newSFXVolume);
    }

    public void SetVoiceVolume(float newVoiceVolume)
    {
        audioMixer.SetFloat("VoiceVolume", newVoiceVolume);
    }
}
