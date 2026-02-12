using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHolder : ActiveBroadcast
{
    public Minigame AssociatedMinigame;

    public bool AutoSubmit = false;

    public List<UpgradesAbstract> Upgrades;

    private bool Submitted = false;

    public static Dictionary<Minigame, List<UpgradeHolder>> AllUpgradeHolders = new Dictionary<Minigame, List<UpgradeHolder>>();

    public int Priority;

    public void Start()
    {
        if (!AllUpgradeHolders.ContainsKey(AssociatedMinigame)) AllUpgradeHolders[AssociatedMinigame] = new List<UpgradeHolder>();
        AllUpgradeHolders[AssociatedMinigame].Add(this);
        
        if (AutoSubmit)
        {
            SubmitUpgrades();
        } else
        {
            ActivationEvents.AddListener(SubmitUpgrades);
        }
    }

    public static void UnlockAll(Minigame TargetedMinigame)
    {
        foreach (UpgradeHolder holder in AllUpgradeHolders[TargetedMinigame])
        {
            holder.SubmitUpgrades();
        }
    }

    public void SubmitUpgrades()
    {
        if (Submitted) return;
        foreach (UpgradesAbstract upgrade in Upgrades)
        {
            upgrade.Prioirty = Priority;
        }

        UpgradeScreenScript.upgradeScreenScripts[AssociatedMinigame].AddNewUpgrades(Upgrades, !AutoSubmit);
        Submitted = true;
    }
}
