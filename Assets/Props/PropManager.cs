using System.Collections.Generic;
using UnityEngine;

public class PropManager : MonoBehaviour
{
    public static PropManager instance;

    public GameObject[] PropList;

    public static Dictionary<string, int> nameToID = new Dictionary<string, int>();

    public void Awake()
    {
        instance = this;

        int id = 0;
        foreach (GameObject prop in PropList)
        {
            nameToID[prop.GetComponent<CarryableObject>().ObjectName] = id;
            id++;
        }
    }

    public void OnValidate()
    {
        instance = this;
    }
}
