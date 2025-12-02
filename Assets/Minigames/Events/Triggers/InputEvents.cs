using System.Collections.Generic;
using UnityEngine;

public class InputEvents : MonoBehaviour
{
    public List<EventAbstract> ZoomEventsPrefab = new List<EventAbstract>();
    private List<EventAbstract> ZoomEvents = new List<EventAbstract>();

    public void Awake()
    {
        ZoomEvents = EventListInstantiate(ZoomEventsPrefab);
    }

    public List<EventAbstract> EventListInstantiate(List<EventAbstract> originalList)
    {
        List<EventAbstract> newEventList = new List<EventAbstract>();

        foreach (EventAbstract e in originalList)
        {
            EventAbstract newEvent = Instantiate(e);
            newEvent.HasBeenTriggered = false;
            newEventList.Add(newEvent);
        }

        return newEventList;
    }

    public void ZoomTriggered()
    {
        List<EventAbstract> eventCopy = new List<EventAbstract>(ZoomEvents);
        foreach (EventAbstract possibleEvent in eventCopy)
        {
            if (possibleEvent.ActivationDay != DayInfo.CurrentDay && possibleEvent.ActivationDay != 0)
            {
                ZoomEvents.Remove(possibleEvent);
                continue;
            }

            if (possibleEvent.CheckIfValid(1))
            {
                possibleEvent.Activate();
                ZoomEvents.Remove(possibleEvent);
            }
        }
    }
}
