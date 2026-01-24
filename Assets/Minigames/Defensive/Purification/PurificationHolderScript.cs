using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PurificationHolderScript : MonoBehaviour
{
    public string SetName;

    public PurificationLevelPacksSO AssociatedLevels;
    public List<BroadcastStruct> HallucinationResets;

    public static Dictionary<string, PurificationHolderScript> LevelHolders;

    private void Awake()
    {
        if (LevelHolders == null) LevelHolders = new Dictionary<string, PurificationHolderScript>();
        LevelHolders.Add(SetName.ToLower(), this);
    }


    public static void BroadcastLevelStart(string setName)
    {
        if (!LevelHolders.ContainsKey(setName)) return;
        LevelHolders[setName.ToLower()].LevelStart();
    }

    public void LevelStart()
    {
        PurificationGameScript.associatedLevelHolder = this;
        PurificationGameScript.SetLevelSet(AssociatedLevels);
    }

}
