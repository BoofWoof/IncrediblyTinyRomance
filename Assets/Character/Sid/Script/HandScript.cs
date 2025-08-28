using UnityEngine;

[ExecuteInEditMode]
public class HandScript : MonoBehaviour
{
    public GameObject[] PossibleObjects;
    public int ObjectIDToSpawn;

    public GameObject SpawnedObject;

    [ContextMenu("Spawn In Hand")] // Adds right-click menu option
    public void SpawnInHandInterface()
    {
        SpawnInHand(ObjectIDToSpawn);
    }

    public void SpawnInHand(int ID)
    {
        if (PossibleObjects == null || PossibleObjects.Length == 0)
        {
            Debug.LogWarning("No possible objects assigned.");
            return;
        }
        if (ObjectIDToSpawn < 0 || ObjectIDToSpawn >= PossibleObjects.Length)
        {
            Debug.LogWarning("Invalid object ID.");
            return;
        }

        SpawnedObject = Instantiate(PossibleObjects[ID]);
        SpawnedObject.transform.parent = transform;
        SpawnedObject.transform.localPosition = Vector3.zero;
        SpawnedObject.transform.localRotation = Quaternion.identity;
        //newObject.transform.localScale = Vector3.one;

        #if UNITY_EDITOR
                UnityEditor.Undo.RegisterCreatedObjectUndo(SpawnedObject, "Spawn Object In Hand");
        #endif
    }

    public void Activate()
    {
        SpawnedObject.GetComponent<CarryableObject>().Activate();
    }

    public void DestroyHeldObject()
    {
        Destroy(SpawnedObject);
    }

    public void ReleaseHandObject(int releaseIdx)
    {
        SpawnedObject.GetComponent<CarryableObject>().GoTo(releaseIdx);
    }

    public void PickupHandObject(string objectName)
    {
        CarryableObject getCarryable = CarryableObject.GetCarryableObject(objectName);
        if (getCarryable == null) {
            Debug.Log("No carryable object with this name.");
            return;
        }
        SpawnedObject = getCarryable.gameObject;

        Debug.Log(SpawnedObject);

        SpawnedObject.transform.parent = transform;
        SpawnedObject.transform.localPosition = Vector3.zero;
        SpawnedObject.transform.localRotation = Quaternion.identity;
    }
}
