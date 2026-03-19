using System;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public AudioMixer audioMixer;
    [Header("Toggles")]
    [Header("QuestVisiblity")]
    public static bool QuestVisibility = true;

    [Header("SubtitleVisiblity")]
    public static bool SubtitleVisiblity = true;

    [Header("Sliders")]
    [Header("MouseSensitivity")]
    public static float MouseSensitivity = 1;

    [Header("TextSpeed")]
    public static float TextSpeed = 1;

    [Header("VibrationIntensity")]
    public static float VibrationIntensity = 1;

    [Header("CursorOpacity")]
    public static float CursorOpacity = 0.35f;

    [Header("Volume")]
    [Header("MasterVolume")]
    public static float MasterVolume = 1;

    [Header("MusicVolume")]
    public static float MusicVolume = 1;

    [Header("SFXVolume")]
    public static float SFXVolume = 1;

    [Header("VoiceVolume")]
    public static float VoiceVolume = 1;

    [Header("AmbientVolume")]
    public static float AmbientVolume = 1;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        Load();
    }

    [Serializable]
    public class SettingsSaveData
    {
        public bool QuestVisibility = true;
        public bool SubtitleVisiblity = true;

        public float MouseSensitivity = 1;
        public float TextSpeed = 1;
        public float VibrationIntensity = 1;
        public float CursorOpacity = 0.35f;

        public float MasterVolume = 1;
        public float MusicVolume = 1;
        public float SFXVolume = 1;
        public float VoiceVolume = 1;
        public float AmbientVolume = 1;
    }
    public static void Save()
    {
        SettingsSaveData newSaveData = new SettingsSaveData()
        {
            QuestVisibility=QuestVisibility,
            SubtitleVisiblity=SubtitleVisiblity,
            MouseSensitivity=MouseSensitivity,
            TextSpeed=TextSpeed,
            VibrationIntensity=VibrationIntensity,
            CursorOpacity=CursorOpacity,
            MasterVolume=MasterVolume,
            MusicVolume=MusicVolume,
            SFXVolume=SFXVolume,
            VoiceVolume=VoiceVolume,
            AmbientVolume=AmbientVolume
        };

        string json = JsonUtility.ToJson(newSaveData, true);
        string path = Application.persistentDataPath + "/settings.json";

        File.WriteAllText(path, json);
        Debug.Log($"Settings saved to: {path}");
    }

    public void Load()
    {
        string path  = Application.persistentDataPath + "/settings.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            SettingsSaveData saveData = JsonUtility.FromJson<SettingsSaveData>(json);

            SetQuestVisiblity(saveData.QuestVisibility);
            SetSubtitleVisiblity(saveData.SubtitleVisiblity);

            SetMouseSensitivity(saveData.MouseSensitivity);
            SetTextSpeed(saveData.TextSpeed);
            SetVibrationIntensity(saveData.VibrationIntensity);
            SetCursorOpacity(saveData.CursorOpacity);

            SetMasterVolume(saveData.MasterVolume);
            SetMusicVolume(saveData.MusicVolume);
            SetSFXVolume(saveData.SFXVolume);
            SetVoiceVolume(saveData.VoiceVolume);
            SetAmbientVolume(saveData.AmbientVolume);
        }
    }
    public static void SetQuestVisiblity(bool questVisibility)
    {
        QuestVisibility = questVisibility;

        if (HudScript.instance == null) return;

        HudScript.SetQuestVisiblity(questVisibility);
    }
    public static void SetSubtitleVisiblity(bool subtitleVisiblity)
    {
        SubtitleVisiblity = subtitleVisiblity;

        if (HudScript.instance == null) return;

        HudScript.SetSubtitleVisiblity(subtitleVisiblity);
    }

    public static void SetMouseSensitivity(float newSensitivity)
    {
        MouseSensitivity = newSensitivity;
        PlayerCam.SetMouseSensitivity(newSensitivity);
    }
    public static void SetTextSpeed(float textSpeed)
    {
        TextSpeed = textSpeed;
        MessagingVariables.SetTimeDivider = textSpeed;
    }

    public static void SetVibrationIntensity(float newIntensity)
    {
        VibrationIntensity = newIntensity;
        MoveCamera.SetVibrationIntensity(newIntensity);
    }

    public static void SetCursorOpacity(float newCursorOpacity)
    {
        CursorOpacity = newCursorOpacity;

        if (HudScript.instance == null) return;

        HudScript.SetReticleOpacity(newCursorOpacity);
    }

    public static void SetMasterVolume(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
        float newVolume = LinearToDB(newMasterVolume);
        instance.audioMixer.SetFloat("MasterVolume", newVolume);
    }

    public static void SetMusicVolume(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
        float newVolume = LinearToDB(newMusicVolume);
        instance.audioMixer.SetFloat("MusicVolume", newVolume);
    }

    public static void SetSFXVolume(float newSFXVolume)
    {
        SFXVolume = newSFXVolume;
        float newVolume = LinearToDB(newSFXVolume);
        instance.audioMixer.SetFloat("SFXVolume", newVolume);
    }

    public static void SetVoiceVolume(float newVoiceVolume)
    {
        VoiceVolume = newVoiceVolume;
        float newVolume = LinearToDB(newVoiceVolume);
        instance.audioMixer.SetFloat("VoiceVolume", newVolume);
    }
    public static void SetAmbientVolume(float newAmbientVolume)
    {
        AmbientVolume = newAmbientVolume;
        float newVolume = LinearToDB(newAmbientVolume);
        instance.audioMixer.SetFloat("AmbientVolume", newVolume);
    }

    public static float LinearToDB(float inputValue)
    {
        return Mathf.Log10(inputValue) * 20f;
    }
}
