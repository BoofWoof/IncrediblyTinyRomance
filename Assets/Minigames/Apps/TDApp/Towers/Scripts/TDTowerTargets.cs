using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDTowerTargets : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();

    internal void CheckDestroyed()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i] == null ) targets.Remove(targets[i]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "TDEnemy")
        {
            targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TDEnemy")
        {
            targets.Remove(other.gameObject);
        }
    }
}
