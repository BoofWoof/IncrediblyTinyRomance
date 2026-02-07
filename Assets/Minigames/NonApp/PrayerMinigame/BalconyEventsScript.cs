using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class BalconyEventsScript : MonoBehaviour
{
    public GameObject PrayerScreen;

    public bool BalconyActivated = false;
    private bool OnBalcony = false;

    public List<PrayerStatueScript> PrayerStatues = new List<PrayerStatueScript>();

    public AudioSource RaiseAudio;
    public AudioSource LowerAudio;

    public bool Raised = false;

    public static BalconyEventsScript instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Lua.RegisterFunction("StartPrayerSystem", this, SymbolExtensions.GetMethodInfo(() => StartSystem()));
    }

    private void Update()
    {
        if (!BalconyActivated && OnBalcony)
        {
            if (
                QuestLog.GetQuestState("A View") == QuestState.Active
                )
            {
                QuestManager.IncrementQuest();
                MessageQueue.addDialogue("FirstAriesSpotting");
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
        if (BalconyActivated) return;

        ActiveBroadcast.BroadcastActivation("PrayerArchive");

        BalconyActivated = true;
        if (OnBalcony && !Raised)
        {
            RaiseStatues();
        }
    }
    public void StopSystem()
    {
        if (!BalconyActivated) return;
        BalconyActivated = false;
        if (Raised)
        {
            LowerStatues();
        }
    }
    public void RaiseStatues()
    {
        Raised = true;
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
        Raised = false;
        RaiseAudio.Stop();
        LowerAudio.Stop();
        LowerAudio.Play();
        foreach (PrayerStatueScript prayerStatueScript in PrayerStatues)
        {
            prayerStatueScript.StatueOff();
        }
    }
}
