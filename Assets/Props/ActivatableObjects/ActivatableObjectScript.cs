using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivatableObjectScript : MonoBehaviour
{
    public float activeDelay = 0f;

    public UnityEvent onActivate;

    public bool ObjectEnabled = true;
    public void Activate()
    {
        if (!ObjectEnabled) return;

        StartCoroutine(ActivationWait());
    }

    IEnumerator ActivationWait()
    {
        yield return new WaitForSeconds(activeDelay);

        // Define what happens when the object is activated
        Debug.Log(gameObject.name + " has been activated!");
        // You can add more logic for interaction
        onActivate.Invoke();
    }
}
