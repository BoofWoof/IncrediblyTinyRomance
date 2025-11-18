using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EventAbstract : ScriptableObject
{
    private static bool ActivationOnHold = false;
    private static List<EventAbstract> WaitingEvents = new List<EventAbstract>();

    public bool HasBeenTriggered = false;

    public int ActivationDay = 0; //0 is all day.
    public string RequiredQuest = "";
    public float Threshold = 1f;
    public string StartDialogue = "";
    public float DialogueDelay = 0f;

    public List<string> AdditionalActivations = new List<string>();

    public bool CheckIfValid(float Value)
    {
        if (RequiredQuest.Length > 0 && QuestLog.GetQuestState("Visions") != QuestState.Active) return false;
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
        foreach (string activationName in AdditionalActivations)
        {
            ActiveBroadcast.BroadcastActivation(activationName);
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
