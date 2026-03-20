using PixelCrushers;
using System;
using System.Collections.Generic;

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
        EventSaveData saveData = SaveSystem.Deserialize<EventSaveData>(s);

        if (saveData == null) return;

        EventAbstract.TriggeredEventIDs = saveData.TriggeredEventIDs;
    }
}
