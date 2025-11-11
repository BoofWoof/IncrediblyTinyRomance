using Unity.VisualScripting;
using UnityEngine;

public enum Minigame
{
    Visions,
    Stocks,
    Rally,
    Hacking
}

public abstract class UpgradesAbstract : ScriptableObject
{
    public int UpgradeID;

    public string UpgradeName;
    public Sprite UpgradeIcon;

    public string DialogueToTrigger = "";
    public bool CompleteQuest = false;
    public bool ProgressQuest = false;

    [TextArea]
    public string UpgradeDescription;

    [Header("Costs")]
    public float Credits;
    public float FlockRenown;
    public float FoundationRenown;
    public float AssscensssionRenown;
    public float RevolutionRenown;

    public bool UpgradeBought = false;

    public Minigame AssociatedMinigame;

    public bool AutoBuy = false;

    public bool CanBuy()
    {
        if (UpgradeBought) return false;
        if (CurrencyData.Credits < Credits) return false;
        if (CurrencyData.RenownFlock < FlockRenown) return false;
        if (CurrencyData.RenownFoundation < FoundationRenown) return false;
        if (CurrencyData.RenownAscension < AssscensssionRenown) return false;
        if (CurrencyData.RenownRevolution < RevolutionRenown) return false;
        return true;
    }
    public bool Buy(bool forceBuy = false)
    {
        bool canBuy = CanBuy();
        if (!canBuy && !forceBuy) return canBuy;
        UpgradeBought = true;

        CurrencyData.Credits -= Credits;
        CurrencyData.RenownFlock -= FlockRenown;
        CurrencyData.RenownFoundation -= FoundationRenown;
        CurrencyData.RenownAscension -= AssscensssionRenown;
        CurrencyData.RenownRevolution -= RevolutionRenown;

        OnBuy();

        if (!forceBuy) UpgradeScreenScript.UpgradeBoughtEvent?.Invoke(AssociatedMinigame);

        if(DialogueToTrigger.Length > 0) MessageQueue.addDialogue(DialogueToTrigger);
        if (CompleteQuest) QuestManager.CompleteQuest(QuestManager.currentQuest);
        if (ProgressQuest) QuestManager.IncrementQuest();

        return canBuy;
    }

    public string CostToText()
    {
        int costTypes = 0;
        string outputText = "";

        if(Credits > 0)
        {
            outputText += "<sprite index=1> " + Credits.NumberToString();
            costTypes++;
        }
        if(FlockRenown > 0)
        {
            if (costTypes > 0) outputText += "\n";
            outputText += "<sprite index=0> " + FlockRenown.NumberToString();
            costTypes++;
        }
        if(FoundationRenown > 0)
        {
            if (costTypes > 0) outputText += "\n";
            outputText += "<sprite index=5> " + FoundationRenown.NumberToString();
            costTypes++;
        }
        if(AssscensssionRenown > 0)
        {
            if (costTypes > 0) outputText += "\n";
            outputText += "<sprite index=2> " + AssscensssionRenown.NumberToString();
            costTypes++;
        }
        if(RevolutionRenown > 0)
        {
            if (costTypes > 0) outputText += "\n";
            outputText += "<sprite index=4> " + RevolutionRenown.NumberToString();
        }

        if (outputText.Length <= 1) return "<sprite index=1> 0";

        return outputText;
    }

    public abstract void OnBuy();
}
