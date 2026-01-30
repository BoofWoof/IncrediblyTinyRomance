using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

public class UpgradeScreenScript : MonoBehaviour
{
    public Minigame AssociatedMinigame;
    public Dictionary<Minigame, string> MinigameToString = new Dictionary<Minigame, string>()
    {
        {Minigame.Visions, "vision"}
    };

    public AudioSource UpgradeAudio;

    public List<UpgradesAbstract> Upgrades;
    public List<UpgradesAbstract> UpgradeClones;
    public List<GameObject> UpgradeObjects;
    public int UpgradesVisible;

    public GameObject UpgradeItemPrefab;

    public RectTransform ContentHolder;
    public float InitialContentHeight = 8f;
    public float GapHeight = 4f;
    [HideInInspector] public float ContentHeight = 0f;

    [HideInInspector] public int DisplayedUpgrades = 0;

    public delegate void UpgradeBoughtDelegate(Minigame minigame);
    public static UpgradeBoughtDelegate UpgradeBoughtEvent;

    public static List<UpgradeScreenScript> upgradeScreenScripts = new List<UpgradeScreenScript>();
    public int RevealedUpgrades = 0;

    public GameObject ProgressToUnlockUpgradesText;

    public static bool WaitToOpen = false;

    public void Awake()
    {
        Lua.RegisterFunction("RevealUpgrades", null, SymbolExtensions.GetMethodInfo(() => BroadcastUpgradeReveal("", 0)));
        Lua.RegisterFunction("UpgradeWait", null, SymbolExtensions.GetMethodInfo(() => EnableWaitTrigger()));

        upgradeScreenScripts.Add(this);

        foreach (UpgradesAbstract upgrade in Upgrades)
        {
            UpgradeClones.Add(Instantiate(upgrade));
        }
        int upgradeIdx = 0;
        foreach (UpgradesAbstract upgrade in UpgradeClones)
        {
            if (upgrade.AutoBuy) upgrade.Buy(true);
            else
            {
                upgrade.UpgradeID = upgradeIdx;
                upgradeIdx++;
            }
        }

        FullGenerate();

        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        if(WaitToOpen) (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
        UpgradeBoughtEvent += UpgradeAudioPlay;
        UpgradeItemScript.UpgradesAnimating = 0;
        Refresh();
    }
    public void OnDisable()
    {
        UpgradeBoughtEvent -= UpgradeAudioPlay;

    }

    public static void EnableWaitTrigger()
    {
        WaitToOpen = true;
    }

    public void UpgradeAudioPlay(Minigame minigameUpgraded)
    {
        if (minigameUpgraded != AssociatedMinigame) return;
        UpgradeAudio.Play();
    }

    public static void BroadcastUpgradeReveal(string game, float quantity)
    {
        foreach (UpgradeScreenScript script in upgradeScreenScripts)
        {
            script.IncreaseUpgradeReveal(game, (int)quantity);
        }
    }

    public void IncreaseUpgradeReveal(string game, int quantity)
    {
        Debug.Log("Reveal " + game + " " + quantity.ToString());
        if (MinigameToString[AssociatedMinigame].ToLower() != game.ToLower()) return;
        RevealedUpgrades += quantity;
        Refresh();
    }

    public void FullGenerate()
    {
        foreach (UpgradesAbstract upgrade in UpgradeClones)
        {
            if (DisplayedUpgrades >= UpgradesVisible) break;
            if (RevealedUpgrades <= upgrade.UpgradeID) break;
            if (upgrade.UpgradeBought) continue;
            AddUpgrade(upgrade);
        }
        ProgressToUnlockUpgradesText?.SetActive(DisplayedUpgrades == 0);
    }
    public void BoughtGenerate()
    {
        foreach (UpgradesAbstract upgrade in UpgradeClones)
        {
            if (!upgrade.UpgradeBought) continue;
            AddUpgrade(upgrade);
        }
    }

    public void Clear()
    {
        foreach (GameObject upgradeObject in UpgradeObjects)
        {
            Destroy(upgradeObject);
        }
        UpgradeObjects.Clear();
        DisplayedUpgrades = 0;
        ContentHeight = 0;
    }

    public void Refresh()
    {
        Clear();
        FullGenerate();
    }
    public void BoughtRefresh()
    {
        Clear();
        BoughtGenerate();
    }

    public void AddUpgrade(UpgradesAbstract newUpgrade)
    {
        DisplayedUpgrades++;

        GameObject newUpgradeObject = Instantiate(UpgradeItemPrefab, ContentHolder);
        UpgradeObjects.Add(newUpgradeObject);
        newUpgradeObject.GetComponent<UpgradeItemScript>().SetUpgrade(newUpgrade);
        newUpgradeObject.GetComponent<UpgradeItemScript>().SetSource(this);
        newUpgradeObject.GetComponent<UpgradeItemScript>().AssociatedUpgrade.AssociatedMinigame = AssociatedMinigame;

        RectTransform rect = newUpgradeObject.GetComponent<RectTransform>();
        rect.localPosition = Vector3.zero;
        ContentHeight += rect.sizeDelta.y + GapHeight;

        ContentHolder.sizeDelta = new Vector2(ContentHolder.sizeDelta.x, ContentHeight + InitialContentHeight);
    }
}
