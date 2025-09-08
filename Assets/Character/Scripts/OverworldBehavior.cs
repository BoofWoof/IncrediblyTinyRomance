using System.Collections.Generic;
using UnityEngine;

public class OverworldBehavior : MonoBehaviour
{
    public CharacterSpeechScript NameSource;

    public static List<OverworldBehavior> OverworldBehaviors = new List<OverworldBehavior>();

    public void Awake()
    {
        OverworldBehaviors.Add(this);
    }

    public void OnDestroy()
    {
        OverworldBehaviors.Remove(this);
    }

    virtual public void ExecuteBehavior(string submitName, string behavior)
    {
    }
}
