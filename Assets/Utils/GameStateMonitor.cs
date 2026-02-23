

using System.Collections.Generic;
using UnityEngine;

public static class GameStateMonitor
{
    private static bool _ForceEventActive = false;
    public static bool ForceEventActive {
        get { return _ForceEventActive; }
        set {
            _ForceEventActive = value;
            CheckEventChange();
        }
    }
    public static void SetForceEvent()
    {
        ForceEventActive = true;
    }
    public static void ReleaseForceEvent()
    {
        ForceEventActive = false;
    }

    private static List<MonoBehaviour> ActiveSpeakingSource = new List<MonoBehaviour>();
    public static void AddSpeakingSource(MonoBehaviour target)
    {
        ActiveSpeakingSource.Add(target);
        CheckEventChange();
    }
    public static void RemoveSpeakingSource(MonoBehaviour target)
    {
        ActiveSpeakingSource.Remove(target);
        CheckEventChange();
    }

    private static bool _ActivePrayer = false;
    public static bool ActivePrayer
    {
        get { return _ActivePrayer; }
        set
        {
            _ActivePrayer = value;
            CheckEventChange();
        }
    }

    public static bool _DangerActive = false;
    public static bool DangerActive
    {
        get { return _DangerActive; }
        set
        {
            _DangerActive = value;
            CheckEventChange();
        }
    }

    public delegate void OnEventChangeDelegate(bool EventActive);
    public static OnEventChangeDelegate OnEventChange;
    private static bool PrevEventActive = false;

    public static void CheckEventChange()
    {
        bool NewEventActive = isEventActive();

        if(NewEventActive != PrevEventActive) OnEventChange?.Invoke(NewEventActive);

        PrevEventActive = NewEventActive;
        PrintGameState();
    }

    public static bool isEventActive()
    {
        return (ActiveSpeakingSource.Count > 0) || (ActivePrayer) || (DangerActive) || ForceEventActive;
    }

    public static void PrintGameState()
    {
        Debug.Log($"The following is the current game state. ForceEvent: {ForceEventActive} DangerActive: {DangerActive} ActivePrayer: {ActivePrayer} ActiveSpeakingSource: {ActiveSpeakingSource.Count}");
    }
}
