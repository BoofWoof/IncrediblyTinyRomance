using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public struct BroadcastStruct
{
    public string BroadcastName;
    public bool UseValue;
    public float BroadcastValue;
    public bool UseString;
    public string BroadcastString;
}
public class ActiveBroadcast : MonoBehaviour
{
    public bool StartActive = true;
    public string ActiveItemName;

    public UnityEvent ActivationEvents;
    public UnityEvent<float> ActivationWithValueEvents;
    public UnityEvent<string> ActivationWithStringEvents;

    static Dictionary<string, ActiveBroadcast> ActiveBroadcastItems = new Dictionary<string, ActiveBroadcast>();

    void Awake()
    {
        gameObject.SetActive(StartActive);
        ActiveBroadcastItems[ActiveItemName.ToLower()] = this;

        Lua.RegisterFunction("BroadcastActivate", null, SymbolExtensions.GetMethodInfo(() => BroadcastActivation("")));
    }

    public static void BroadcastActivation(string triggerName)
    {
        ActiveBroadcastItems[triggerName.ToLower()].ActivationEvents?.Invoke();
    }

    public static void BroadcastActivation(BroadcastStruct broadcastData)
    {
        if (broadcastData.BroadcastName.Length == 0) return;
        if (broadcastData.UseValue)
        {
            ActiveBroadcastItems[broadcastData.BroadcastName.ToLower()].ActivationWithValueEvents?.Invoke(broadcastData.BroadcastValue);
        } else if (broadcastData.UseString)
        {
            ActiveBroadcastItems[broadcastData.BroadcastName.ToLower()].ActivationWithStringEvents?.Invoke(broadcastData.BroadcastString);
        } else
        {
            ActiveBroadcastItems[broadcastData.BroadcastName.ToLower()].ActivationEvents?.Invoke();
        }
    }
    public static void BroadcastWithValueActivation(string triggerName, float value)
    {
        ActiveBroadcastItems[triggerName.ToLower()].ActivationWithValueEvents?.Invoke(value);
    }
    public static void BroadcastWithStringActivation(string triggerName, string activationName)
    {
        ActiveBroadcastItems[triggerName.ToLower()].ActivationWithStringEvents?.Invoke(activationName);
    }


}
