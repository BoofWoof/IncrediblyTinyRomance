using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EventAbstract : ScriptableObject
{
    private static bool ActivationOnHold = false;
    private static List<EventAbstract> WaitingEvents = new List<EventAbstract>();

    public bool HasBeenTriggered = false;

    public int ActivationDay = 0; //0 is all days.
    public string RequiredQuest = "";
    public float Threshold = 1f;
    public string StartDialogue = "";
    public float DialogueDelay = 0f;

    public List<BroadcastStruct> AdditionalActivations = new List<BroadcastStruct>();

    public bool CheckIfValid(float Value)
    {
        if (RequiredQuest.Length > 0 && QuestLog.GetQuestState(RequiredQuest) != QuestState.Active) return false;
        return Value >= Threshold;
    }

    public void Activate()
    {
        if (ActivationOnHold)
        {
            WaitingEvents.Add(this);
            return;
        }


        if (HasBeenTriggered) return;
        HasBeenTriggered = true;

        ActivateAdditionalConnections();
        if (StartDialogue.Length > 0) MessageQueue.addDialogue(StartDialogue, DialogueDelay);

        OnActivate();
    }

    public virtual void ActivateAdditionalConnections()
    {
        foreach (BroadcastStruct broadcastData in AdditionalActivations)
        {
            ActiveBroadcast.BroadcastActivation(broadcastData);
        }
    }

    public static void FreeHold()
    {
        ActivationOnHold = false;
        if (WaitingEvents.Count <= 0) return;

        EventAbstract nextEvent = WaitingEvents[0];
        WaitingEvents.RemoveAt(0);
        nextEvent.Activate();
    }

    public abstract void OnActivate();
}
