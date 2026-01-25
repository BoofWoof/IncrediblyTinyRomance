using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class OverworldBehavior : MonoBehaviour
{
    public CharacterSpeechScript NameSource;

    public static List<OverworldBehavior> OverworldBehaviors = new List<OverworldBehavior>();

    public void Awake()
    {
        OverworldBehaviors.Add(this);
        Lua.RegisterFunction("AriesBehavior", null, SymbolExtensions.GetMethodInfo(() => AriesBehavior("")));
        Lua.RegisterFunction("AriesWaitBehavior", null, SymbolExtensions.GetMethodInfo(() => AriesWaitBehavior("", 0f)));
    }

    public void OnDestroy()
    {
        OverworldBehaviors.Remove(this);
    }

    public static void BroadcastBehaviors(string name, string behavior, float wait = 0f)
    {
        foreach (OverworldBehavior overworldBehavior in OverworldBehaviors)
        {
            overworldBehavior.ExecuteBehavior(name, behavior, wait);
        }
    }

    virtual public void ExecuteBehavior(string submitName, string behavior, float wait)
    {
    }

    public static void AriesBehavior(string behavior)
    {
        BroadcastBehaviors("MacroAries", behavior, 0);
    }

    public static void AriesWaitBehavior(string behavior, float Wait)
    {
        BroadcastBehaviors("MacroAries", behavior, Wait);
    }
}
