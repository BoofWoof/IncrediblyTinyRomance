using DS;
using DS.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalconyEventsScript : MonoBehaviour
{
    public GameObject PrayerScreen;

    public DialogueOptionsVariable MiloStartCheck;

    private bool OnBalcony = false;

    public DSDialogue PrayerTutorial;

    private void Start()
    {
        PrayerScreen.SetActive(false);
    }

    private void Update()
    {
        if (OnBalcony)
        {
            if (
                    DSMemory.OptionMemory.ContainsKey(MiloStartCheck.uniqueID) &&
                    DSMemory.OptionMemory[MiloStartCheck.uniqueID] == MiloStartCheck.StateUuids[1])
            {
                DSMemory.OptionMemory.Remove(MiloStartCheck.uniqueID);
                PrayerTutorial.SubmitDialogue();
                PrayerTutorial.OnMessageComplete += StartPrayers;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        OnBalcony = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        OnBalcony = false;
    }

    public void StartPrayers()
    {
        PrayerScript.IncreaseAnger = true;
        PrayerScreen.SetActive(true);
    }
}
