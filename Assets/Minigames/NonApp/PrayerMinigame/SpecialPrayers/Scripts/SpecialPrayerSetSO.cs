using System;
using UnityEngine;

[Serializable]
public struct SpecialPrayerData
{
    public string Option;
    public string AuthorName;
    public bool GoodPrayer;
    public VoiceLineSO SpecialResponse;
    public string TriggerDialogue;
    public string TriggerActivation;
}

[CreateAssetMenu(fileName = "SpecialPrayerSetSO", menuName = "SpecialPrayerSetSO")]
public class SpecialPrayerSetSO : ScriptableObject
{
    public string ID;
    public string SetName;
    public bool ForceSelection;
    public SpecialPrayerData[] PrayerOptions = new SpecialPrayerData[3];

    public void Awake()
    {
        if(ID == null) ID = System.Guid.NewGuid().ToString();
    }
}
