using System;
using UnityEngine;

[Serializable]
public struct SpecialPrayerData
{
    public string Option;
    public bool GoodPrayer;
    public VoiceLineSO SpecialResponse;
    public string TriggerDialogue;
    public string TriggerActivation;
}

[CreateAssetMenu(fileName = "SpecialPrayerSetSO", menuName = "SpecialPrayerSetSO")]
public class SpecialPrayerSetSO : ScriptableObject
{
    public string SetName;
    public SpecialPrayerData[] PrayerOptions = new SpecialPrayerData[3];
}
