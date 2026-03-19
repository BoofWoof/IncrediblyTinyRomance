using PixelCrushers;
using System;
using System.Collections.Generic;
using UnityEngine.LightTransport;

public class EventSaver : Saver
{
    [Serializable]
    public class EventSaveData
    {
        public List<string> TriggeredEventIDs = new List<string>();
    }
    public override string RecordData()
    {
        EventSaveData newSaveData = new EventSaveData()
        {
            TriggeredEventIDs = EventAbstract.TriggeredEventIDs
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        EventAbstract.TriggeredEventIDs = SaveSystem.Deserialize<EventSaveData>(s).TriggeredEventIDs;
    }
}
