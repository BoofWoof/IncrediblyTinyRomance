using System.Collections.Generic;
using UnityEngine;

public class PurificationHolderScript : MonoBehaviour
{
    public string SetName;

    public PurificationLevelPacksSO AssociatedLevels;
    public List<BroadcastStruct> HallucinationResets;

    public static Dictionary<string, PurificationHolderScript> LevelHolders;

    private void Awake()
    {
        LevelHolders = new Dictionary<string, PurificationHolderScript>();
    }

    public void Start()
    {
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
        Debug.Log(AssociatedLevels);
        PurificationGameScript.SetLevelSet(AssociatedLevels);
    }

}
