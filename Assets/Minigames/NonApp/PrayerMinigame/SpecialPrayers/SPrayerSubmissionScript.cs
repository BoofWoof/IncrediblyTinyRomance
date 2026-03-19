using PixelCrushers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SPrayerSubmissionScript : Saver
{
    public List<SpecialPrayerSetSO> PrayerToSubmit;

    public static List<SpecialPrayerSetSO> WaitingForcedPrayers;
    public static List<SpecialPrayerData> WaitingSpecialPrayers;

    public static List<string> SentPrayerIDs = new List<string>();

    public delegate void NewPrayerDelegate();
    public static NewPrayerDelegate OnNewForcedPrayers;
    public static NewPrayerDelegate OnNewSpecialPrayer;

    public bool Submitted = false;

    public bool SetSubmitterAsTopicTarget = false;

    new public void Awake()
    {
        base.Awake();
        WaitingForcedPrayers = new List<SpecialPrayerSetSO>();
        WaitingSpecialPrayers = new List<SpecialPrayerData>();
        SentPrayerIDs = new List<string>();
        Submitted = false;
    }

    public void SubmitPrayersToQueue()
    {
        if(Submitted) return;
        foreach (SpecialPrayerSetSO prayerSet in PrayerToSubmit)
        {
            if (SentPrayerIDs.Contains(prayerSet.ID)) return;

            SpecialPrayerSetSO instantiatedPrayer = Instantiate(prayerSet);

            foreach (SpecialPrayerData specialPrayerData in instantiatedPrayer.PrayerOptions)
            {
                specialPrayerData.ParentID = prayerSet.ID;
            }

            if (SetSubmitterAsTopicTarget)
            {
                foreach (SpecialPrayerData specialPrayerData in instantiatedPrayer.PrayerOptions)
                {
                    Debug.Log("SETTING PRAYER");
                    Debug.Log(transform);
                    specialPrayerData.TopicTarget = transform;
                }
            }

            if (instantiatedPrayer.ForceSelection)
            {
                WaitingForcedPrayers.Add(instantiatedPrayer);
                OnNewForcedPrayers?.Invoke();
            } else
            {
                foreach (SpecialPrayerData specialPrayerData in instantiatedPrayer.PrayerOptions)
                {
                    WaitingSpecialPrayers.Add(specialPrayerData);
                }
                OnNewSpecialPrayer?.Invoke();
            }
        }
        Submitted = true;
    }

    [Serializable]
    public class PSubmissionSaveData
    {
        public bool Submitted = false;
    }

    public override string RecordData()
    {
        PSubmissionSaveData newSaveData = new PSubmissionSaveData()
        {
            Submitted = Submitted
        };
        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        if (SaveSystem.Deserialize<PSubmissionSaveData>(s).Submitted) SubmitPrayersToQueue();
    }
}
