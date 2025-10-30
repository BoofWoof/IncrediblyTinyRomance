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
    }

    public void OnDestroy()
    {
        OverworldBehaviors.Remove(this);
    }

    public static void BroadcastBehaviors(string name, string behavior)
    {
        foreach (OverworldBehavior overworldBehavior in OverworldBehaviors)
        {
            overworldBehavior.ExecuteBehavior(name, behavior);
        }
    }

    virtual public void ExecuteBehavior(string submitName, string behavior)
    {
    }

    public static void AriesBehavior(string behavior)
    {
        BroadcastBehaviors("MacroAries", behavior);
    }
}
