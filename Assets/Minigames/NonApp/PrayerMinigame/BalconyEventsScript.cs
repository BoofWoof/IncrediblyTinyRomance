using DS;
using DS.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalconyEventsScript : MonoBehaviour
{
    public GameObject PrayerScreen;

    public DialogueOptionsVariable MiloStartCheck;

    public DSDialogue PrayerTutorial;

    private bool BalconyActivated = false;
    private bool OnBalcony = false;

    public List<PrayerStatueScript> PrayerStatues = new List<PrayerStatueScript>();

    public AudioSource RaiseAudio;
    public AudioSource LowerAudio;

    private void Update()
    {
        if (!BalconyActivated && OnBalcony)
        {
            if (
                    DSMemory.OptionMemory.ContainsKey(MiloStartCheck.uniqueID) &&
                    DSMemory.OptionMemory[MiloStartCheck.uniqueID] == MiloStartCheck.StateUuids[1])
            {
                DSMemory.OptionMemory.Remove(MiloStartCheck.uniqueID);
                PrayerTutorial.SubmitDialogue();
                PrayerTutorial.OnMessageComplete += StartSystem;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        OnBalcony = true;
        if (BalconyActivated)
        {
            RaiseStatues();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        OnBalcony = false;
        if (BalconyActivated)
        {
            LowerStatues();
        }
    }

    public void StartSystem()
    {
        PrayerScript.IncreaseAnger = true;
        BalconyActivated = true;
        if (OnBalcony)
        {
            RaiseStatues();
        }
    }

    public void RaiseStatues()
    {
        RaiseAudio.Stop();
        LowerAudio.Stop();
        RaiseAudio.Play();
        foreach (PrayerStatueScript prayerStatueScript in PrayerStatues)
        {
            prayerStatueScript.StatueOn();
        }
    }

    public void LowerStatues()
    {
        RaiseAudio.Stop();
        LowerAudio.Stop();
        LowerAudio.Play();
        foreach (PrayerStatueScript prayerStatueScript in PrayerStatues)
        {
            prayerStatueScript.StatueOff();
        }
    }
}
