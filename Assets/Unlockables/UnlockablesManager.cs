using PixelCrushers.DialogueSystem;
using System.Collections;
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
    public static List<string> UnlockedPostersList = new List<string>();

    public static UnlockablesManager instance;

    public static int PostersUnlockedCount;

    public AudioSource UnlockSound;

    private static List<string> PostersToUnlock = new List<string>();
    private static bool UnlockRunning = false;

    public void Awake()
    {
        instance = this;

        PostersList = InitialPostersList;

        PostersDict = new Dictionary<string, OfficePoster>();

        Lua.RegisterFunction("UnlockPortrait", null, SymbolExtensions.GetMethodInfo(() => UnlockPortrait("")));
    }

    public void Start()
    {
        foreach (OfficePoster poster in PostersList)
        {
            PostersDict.Add(poster.Name, poster);
        }
    }

    public static void UnlockPortrait(string posterName)
    {
        UnlockPortrait(posterName, false);
    }

    public static void LoadFromList(List<string> loadList)
    {
        foreach(string posterName in loadList)
        {
            UnlockPortrait(posterName, true);
        }
    }

    public static void UnlockPortrait(string posterName, bool skipAnimation = false)
    {
        if (!PostersDict.ContainsKey(posterName))
        {
            Debug.LogError("Poster name not found: " + posterName);
            return;
        }

        if (PostersDict[posterName].Unlocked) return;

        PostersUnlockedCount++;

        Debug.Log("Unlocking: " + posterName);

        PostersDict[posterName].Unlocked = true;
        UnlockedPostersList.Add(posterName);

        if (skipAnimation) return;

        PostersToUnlock.Add(posterName);

        if (UnlockRunning) return;

        instance.StartCoroutine(instance.UnlockPosterAnimation());
    }

    public IEnumerator UnlockPosterAnimation()
    {
        UnlockRunning = true;
        for (int i = 0; i < PostersToUnlock.Count; i++)
        {
            UnlockSound.Play();
            string posterName = PostersToUnlock[i];
            instance.GetComponentInChildren<Image>().sprite = PostersDict[posterName].Image;
            instance.GetComponent<Animator>().SetTrigger("UnlockArt");
            yield return new WaitForSeconds(5f);
        }
        PostersToUnlock.Clear();
        UnlockRunning = false;
    }

    public static void UnlockAllPortraits()
    {
        foreach (string posterName in PostersDict.Keys)
        {
            UnlockPortrait(posterName, true);
        }
    }
}
