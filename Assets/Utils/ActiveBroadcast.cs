using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActiveBroadcast : MonoBehaviour
{
    public bool StartActive = true;
    public string ActiveItemName;

    public UnityEvent ActivationEvents;

    static Dictionary<string, ActiveBroadcast> ActiveBroadcastItems = new Dictionary<string, ActiveBroadcast>();

    void Awake()
    {
        gameObject.SetActive(StartActive);
        ActiveBroadcastItems[ActiveItemName.ToLower()] = this;
    }

    public static void BroadcastActivation(string triggerName)
    {
        ActiveBroadcastItems[triggerName.ToLower()].ActivationEvents?.Invoke();
    }


}
