using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
    [Header("Toggles")]
    [Header("QuestVisiblity")]
    public Toggle QuestVisibilityToggle;

    [Header("SubtitleVisiblity")]
    public Toggle SubtitleVisiblityToggle;

    [Header("Sliders")]
    [Header("MouseSensitivity")]
    public Slider MouseSensitivitySlider;

    [Header("TextSpeed")]
    public Slider TextSpeedSlider;

    [Header("VibrationIntensity")]
    public Slider VibrationIntensitySlider;

    [Header("CursorOpacity")]
    public Slider CursorOpacitySlider;

    [Header("Volume")]
    [Header("MasterVolume")]
    public Slider MasterVolumeSlider;

    [Header("MusicVolume")]
    public Slider MusicVolumeSlider;

    [Header("SFXVolume")]
    public Slider SFXVolumeSlider;

    [Header("VoiceVolume")]
    public Slider VoiceVolumeSlider;

    [Header("AmbientVolume")]
    public Slider AmbientVolumeSlider;

    public void Awake()
    {
        QuestVisibilityToggle.isOn = SettingsManager.QuestVisibility;
        SubtitleVisiblityToggle.isOn = SettingsManager.SubtitleVisiblity;

        MouseSensitivitySlider.value = SettingsManager.MouseSensitivity;
        TextSpeedSlider.value = SettingsManager.TextSpeed;
        VibrationIntensitySlider.value = SettingsManager.VibrationIntensity;
        CursorOpacitySlider.value = SettingsManager.CursorOpacity;

        MasterVolumeSlider.value = SettingsManager.MasterVolume;
        MusicVolumeSlider.value = SettingsManager.MusicVolume;
        SFXVolumeSlider.value = SettingsManager.SFXVolume;
        VoiceVolumeSlider.value = SettingsManager.VoiceVolume;
        AmbientVolumeSlider.value = SettingsManager.AmbientVolume;
    }

    public void OnDisable()
    {
        SettingsManager.Save();
    }

    public void SetMouseSensitivity(float newSensitivity)
    {
        SettingsManager.SetMouseSensitivity(newSensitivity);
    }
    public void SetTextSpeed(float textSpeed)
    {
        SettingsManager.SetTextSpeed(textSpeed);
    }

    public void SetVibrationIntensity(float newIntensity)
    {
        SettingsManager.SetVibrationIntensity(newIntensity);
    }

    public void SetCursorOpacity(float newCursorOpacity)
    {
        SettingsManager.SetVibrationIntensity(newCursorOpacity);
    }
    public void SetQuestVisiblity(bool questVisibility)
    {
        SettingsManager.SetQuestVisiblity(questVisibility);
    }
    public void SetSubtitleVisiblity(bool subtitleVisiblity)
    {
        SettingsManager.SetSubtitleVisiblity(subtitleVisiblity);
    }

    public void SetMasterVolume(float newMasterVolume)
    {
        SettingsManager.SetMasterVolume(newMasterVolume);
    }

    public void SetMusicVolume(float newMusicVolume)
    {
        SettingsManager.SetMusicVolume(newMusicVolume);
    }

    public void SetSFXVolume(float newSFXVolume)
    {
        SettingsManager.SetSFXVolume(newSFXVolume);
    }

    public void SetVoiceVolume(float newVoiceVolume)
    {
        SettingsManager.SetVoiceVolume(newVoiceVolume);
    }
    public void SetAmbientVolume(float newAmbientVolume)
    {
        SettingsManager.SetAmbientVolume(newAmbientVolume);
    }
}
