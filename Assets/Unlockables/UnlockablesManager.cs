using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class OfficePoster
{
    public string Name;
    public string Artist;
    public string Description;
    public Sprite Image;
    public bool Unlocked;
}

public class UnlockablesManager: MonoBehaviour
{
    public List<OfficePoster> InitialPostersList;
    public static List<OfficePoster> PostersList;
    public static Dictionary<string, OfficePoster> PostersDict = new Dictionary<string, OfficePoster>();
    public static Dictionary<string, bool> PostersUnlockDict = new Dictionary<string, bool>(); //For saving and loading.

    public static UnlockablesManager instance;

    public void Awake()
    {
        instance = this;

        PostersList = InitialPostersList;

        foreach (OfficePoster poster in PostersList)
        {
            PostersDict.Add(poster.Name, poster);
            PostersUnlockDict.Add(poster.Name, poster.Unlocked);
        }

        Lua.RegisterFunction("UnlockPortrait", null, SymbolExtensions.GetMethodInfo(() => UnlockPortrait("")));
    }

    public static void UnlockPortrait(string posterName)
    {
        if (!PostersDict.ContainsKey(posterName))
        {
            Debug.LogError("Poster name not found: " + posterName);
            return;
        }

        Debug.Log("Unlocking: " + posterName);

        PostersUnlockDict[posterName] = true;
        PostersDict[posterName].Unlocked = true;

        instance.GetComponentInChildren<Image>().sprite = PostersDict[posterName].Image;
        instance.GetComponent<Animator>().SetTrigger("UnlockArt");
    }

    public static void UnlockAllPortraits()
    {
        foreach (string posterName in PostersDict.Keys)
        {
            UnlockPortrait(posterName);
        }
    }
}
