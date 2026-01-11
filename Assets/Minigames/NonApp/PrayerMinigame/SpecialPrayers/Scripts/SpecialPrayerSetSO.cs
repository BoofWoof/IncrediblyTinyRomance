using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpecialPrayerData
{
    public string Option;
    public string AuthorName;
    public bool GoodPrayer;
    public List<VoiceLineSO> SpecialResponseChain;
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
