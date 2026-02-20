using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpecialPrayerData
{
    public string Option;
    public string AuthorName;
    public bool GoodPrayer;
    public List<VoiceLineSO> SpecialResponseChain;
    public string TriggerDialogue;
    public string TriggerActivation;

    public float GetChainTime()
    {
        float totaltime = 0f;
        foreach (VoiceLineSO vo in SpecialResponseChain)
        {
            totaltime += vo.PauseBeforeStart + vo.PauseAfterEnd + vo.AudioData.length;
        }
        return totaltime;
    }
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
