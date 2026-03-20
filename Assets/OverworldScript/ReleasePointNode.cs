using PixelCrushers;
using System;
using UnityEngine;

public class ReleasePointNode : Saver
{
    public CarryableObject heldObject;

    [Serializable]
    public class ReleasePonintSaveData
    {
        public int containedObjectID;
    }
    public override void ApplyData(string s)
    {
        ReleasePonintSaveData saveData = SaveSystem.Deserialize<ReleasePonintSaveData>(s);

        if (saveData == null || saveData.containedObjectID < 0) return;

        GameObject spawnedObject = Instantiate(PropManager.instance.PropList[saveData.containedObjectID]);
        spawnedObject.transform.localScale = Vector3.one;
        spawnedObject.transform.parent = transform;
        spawnedObject.transform.localPosition = Vector3.zero;
        spawnedObject.transform.localRotation = Quaternion.identity;

        spawnedObject.GetComponent<CarryableObject>().CurrentReleaseNode = this;
        heldObject = spawnedObject.GetComponent<CarryableObject>();
    }

    public override string RecordData()
    {
        ReleasePonintSaveData saveData = new ReleasePonintSaveData();
        if(heldObject == null)
        {
            saveData.containedObjectID = -1;
        } else
        {
            saveData.containedObjectID = heldObject.ObjectID;
        }

        return SaveSystem.Serialize(saveData);
    }
}
