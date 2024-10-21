using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivatableObjectScript : MonoBehaviour
{
    public UnityEvent onActivate;
    public void Activate()
    {
        // Define what happens when the object is activated
        Debug.Log(gameObject.name + " has been activated!");
        // You can add more logic for interaction
        onActivate.Invoke();
    }
}
