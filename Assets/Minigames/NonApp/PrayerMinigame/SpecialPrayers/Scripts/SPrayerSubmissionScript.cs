using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class SPrayerSubmissionScript : MonoBehaviour
{
    public List<SpecialPrayerSetSO> PrayerToSubmit;

    public static List<SpecialPrayerSetSO> WaitingForcedPrayers;
    public static List<SpecialPrayerData> WaitingSpecialPrayers;

    public delegate void NewPrayerDelegate();
    public static NewPrayerDelegate OnNewForcedPrayers;
    public static NewPrayerDelegate OnNewSpecialPrayer;

    public bool Submitted = false;

    public void Awake()
    {
        if (WaitingForcedPrayers == null) WaitingForcedPrayers = new List<SpecialPrayerSetSO>();
        if (WaitingSpecialPrayers == null) WaitingSpecialPrayers = new List<SpecialPrayerData>();
        Submitted = false;
    }

    public void SubmitPrayersToQueue()
    {
        if(Submitted) return;
        foreach (SpecialPrayerSetSO prayerSet in PrayerToSubmit)
        {
            SpecialPrayerSetSO instantiatedPrayer = Instantiate(prayerSet);
            if (instantiatedPrayer.ForceSelection)
            {
                WaitingForcedPrayers.Add(instantiatedPrayer);
                OnNewForcedPrayers?.Invoke();
            } else
            {
                foreach (SpecialPrayerData specialPrayerData in prayerSet.PrayerOptions)
                {
                    WaitingSpecialPrayers.Add(specialPrayerData);
                }
                OnNewSpecialPrayer?.Invoke();
            }
        }
        Submitted = true;
    }
}
