using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;

public class VisionEvents : MonoBehaviour
{
    public List<EventAbstract> PuzzleCompleteEventsPrefab = new List<EventAbstract>();
    private List<EventAbstract> PuzzleCompleteEvents = new List<EventAbstract>();

    public List<EventAbstract> UpgradeBoughtEventsPrefab = new List<EventAbstract>();
    private List<EventAbstract> UpgradeBoughtEvents = new List<EventAbstract>();

    public List<EventAbstract> MoneyEarnedEventsPrefab = new List<EventAbstract>();
    private List<EventAbstract> MoneyEarnedEvents = new List<EventAbstract>();

    private void OnEnable()
    {
        TurkPuzzleScript.OnPuzzleComplete += OnPuzzleCompletion;
        UpgradeScreenScript.UpgradeBoughtEvent += OnUpgradeBought;
        CurrencyData.CreditUpdate += OnCreditChange;
    }
    private void OnDisable()
    {
        UpgradeScreenScript.UpgradeBoughtEvent -= OnUpgradeBought;
        TurkPuzzleScript.OnPuzzleComplete -= OnPuzzleCompletion;
        CurrencyData.CreditUpdate -= OnCreditChange;
    }

    public void Awake()
    {
        PuzzleCompleteEvents = EventListInstantiate(PuzzleCompleteEventsPrefab);
        UpgradeBoughtEvents = EventListInstantiate(UpgradeBoughtEventsPrefab);
        MoneyEarnedEvents = EventListInstantiate(MoneyEarnedEventsPrefab);
    }

    public List<EventAbstract> EventListInstantiate(List<EventAbstract> originalList)
    {
        List<EventAbstract> newEventList = new List<EventAbstract>();

        foreach(EventAbstract e in originalList)
        {
            EventAbstract newEvent = Instantiate(e);
            newEvent.HasBeenTriggered = false;
            newEventList.Add(newEvent);
        }

        return newEventList;
    }

    public void OnPuzzleCompletion(int PuzzlesComplete, TurkPuzzleScript puzzleScript)
    {
        int allCount = DialogueLua.GetVariable("PuzzlesCompleted").asInt + 1;
        DialogueLua.SetVariable("PuzzlesCompleted", allCount);

        List<EventAbstract> eventCopy = new List<EventAbstract>(PuzzleCompleteEvents);
        foreach (EventAbstract possibleEvent in eventCopy)
        {
            if (possibleEvent.ActivationDay != DayInfo.CurrentDay && possibleEvent.ActivationDay != 0)
            {
                PuzzleCompleteEvents.Remove(possibleEvent);
                continue;
            }

            if (possibleEvent.CheckIfValid(PuzzlesComplete)) {
                possibleEvent.Activate();
                PuzzleCompleteEvents.Remove(possibleEvent);
            }
        }
    }

    public void OnUpgradeBought(Minigame minigame)
    {
        if (minigame != Minigame.Visions) return;
        int allCount = DialogueLua.GetVariable("PuzzleUpgradesBought").asInt + 1;
        DialogueLua.SetVariable("PuzzleUpgradesBought", allCount);

        List<EventAbstract> eventCopy = new List<EventAbstract>(UpgradeBoughtEvents);
        foreach (EventAbstract possibleEvent in eventCopy)
        {
            if (possibleEvent.ActivationDay != DayInfo.CurrentDay && possibleEvent.ActivationDay != 0)
            {
                UpgradeBoughtEvents.Remove(possibleEvent);
                continue;
            }

            if (possibleEvent.CheckIfValid(allCount))
            {
                possibleEvent.Activate();
                UpgradeBoughtEvents.Remove(possibleEvent);
            }
        }
    }
    public void OnCreditChange(float newValue)
    {
        List<EventAbstract> eventCopy = new List<EventAbstract>(MoneyEarnedEvents);
        foreach (EventAbstract possibleEvent in eventCopy)
        {
            if (possibleEvent.ActivationDay != DayInfo.CurrentDay && possibleEvent.ActivationDay != 0)
            {
                MoneyEarnedEvents.Remove(possibleEvent);
                continue;
            }

            if (possibleEvent.CheckIfValid(newValue))
            {
                possibleEvent.Activate();
                MoneyEarnedEvents.Remove(possibleEvent);
            }
        }
    }
}
