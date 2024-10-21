using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TDNodeScript : MonoBehaviour
{
    public GameObject prefabToInstantiate;
    public GameObject nextNode;
    public GameObject prevNode;

    public bool isStart;
    public bool isEnd;

    public Color defaultColor = Color.white;
    public Color startColor = Color.green;
    public Color endColor = Color.red;
    public Color connectingLine = Color.white;

    // This method will be called when the button is clicked
    public void CreateObject()
    {
        if (prefabToInstantiate != null)
        {
            GameObject newObject = Instantiate(prefabToInstantiate, transform.position, Quaternion.identity);
            newObject.name = prefabToInstantiate.name;

            newObject.transform.parent = transform.parent;
            newObject.transform.localPosition = transform.localPosition;
            newObject.transform.localScale = Vector3.one;
            newObject.transform.localRotation = Quaternion.identity;

            nextNode = newObject;
            newObject.GetComponent<TDNodeScript>().prevNode = gameObject;

            Selection.activeGameObject = newObject;
        }
        else
        {
            Debug.LogWarning("No prefab assigned to instantiate!");
        }
    }

    private void OnDrawGizmos()
    {
        if (nextNode != null)
        {
            GetComponent<Image>().color = defaultColor;
            Gizmos.color = connectingLine; // Set the line color
            Gizmos.DrawLine(transform.position, nextNode.transform.position); // Draw line between the two objects
            isEnd = false;
        }
        else
        {
            GetComponent<Image>().color = endColor;
            isEnd = true;
        }

        if (prevNode == null)
        {
            GetComponent<Image>().color = startColor;
            isStart = true;
        }
        else
        {
            isStart = false;
        }

    }
}
