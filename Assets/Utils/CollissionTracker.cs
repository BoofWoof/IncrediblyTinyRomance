using System.Collections.Generic;
using UnityEngine;

public class CollissionTracker : MonoBehaviour
{
    public string TagName;
    private List<GameObject> collidedObjects = new List<GameObject>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagName))
        {
            collidedObjects.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagName))
        {
            collidedObjects.Remove(other.gameObject);
        }
    }

    public GameObject GetFirstTarget()
    {
        if (collidedObjects.Count > 0)
        {
            return collidedObjects[0];
        }
        return null;
    }
}
