using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpecialPrayerData
{
    public string Option;
    public string AuthorName;
    public bool GoodPrayer;
    [HideInInspector] public List<VoiceLineSO> SpecialResponseChainVL;
    private List<ResourceRequest> SpecialResponseChainRequests;
    public List<string> SpecialResponseChainString;
    public string TriggerDialogue;
    public string TriggerActivation;

    private float _TotalTime = 0;
    private bool _LoadedStarted = false;
    private bool _Loaded = false;

    public float GetChainTime()
    {
        if (!_Loaded) Debug.LogError("Voice lines not loaded yet.");
        return _TotalTime;
    }

    public void StartLoadResponseChain()
    {
        if (_LoadedStarted) return;
        _LoadedStarted = true;

        SpecialResponseChainRequests = new List<ResourceRequest>();

        foreach (string voPath in SpecialResponseChainString)
        {
            SpecialResponseChainRequests.Add(Resources.LoadAsync<VoiceLineSO>(voPath.CleanResourcePath())); // Replace GameObject with your asset type
        }
    }
    public IEnumerator WaitLoadResponseChain()
    {
        if (!_LoadedStarted) yield break;
        if (_Loaded) yield break;
        _Loaded = true;

        _TotalTime = 0;

        SpecialResponseChainVL = new List<VoiceLineSO>();

        foreach (ResourceRequest voRequest in SpecialResponseChainRequests)
        {
            yield return voRequest;

            VoiceLineSO vo = voRequest.asset as VoiceLineSO;

            SpecialResponseChainVL.Add(vo);
            _TotalTime += vo.PauseBeforeStart + vo.PauseAfterEnd + vo.AudioData.length + 0.1f;
        }
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
