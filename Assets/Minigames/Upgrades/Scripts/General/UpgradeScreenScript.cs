using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UpgradeScreenScript : MonoBehaviour
{
    public Minigame AssociatedMinigame;

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

    public void Awake()
    {
        foreach (UpgradesAbstract upgrade in Upgrades)
        {
            UpgradeClones.Add(Instantiate(upgrade));
        }
        foreach (UpgradesAbstract upgrade in UpgradeClones)
        {
            if (upgrade.AutoBuy) upgrade.Buy(true);
        }

        FullGenerate();

        gameObject.SetActive(false);
    }

    public void FullGenerate()
    {
        foreach (UpgradesAbstract upgrade in UpgradeClones)
        {
            if (DisplayedUpgrades >= UpgradesVisible) break;
            if (upgrade.UpgradeBought) continue;
            AddUpgrade(upgrade);
        }
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
        rect.anchoredPosition = Vector2.down * (ContentHeight + InitialContentHeight);
        ContentHeight += rect.sizeDelta.y + GapHeight;

        ContentHolder.sizeDelta = new Vector2(ContentHolder.sizeDelta.x, ContentHeight + InitialContentHeight);
    }
}
