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
}
public class ActiveBroadcast : MonoBehaviour
{
    public bool StartActive = true;
    public string ActiveItemName;

    public UnityEvent ActivationEvents;
    public UnityEvent<float> ActivationWithValueEvents;

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
        if (broadcastData.UseValue)
        {
            ActiveBroadcastItems[broadcastData.BroadcastName.ToLower()].ActivationWithValueEvents?.Invoke(broadcastData.BroadcastValue);
        } else
        {
            ActiveBroadcastItems[broadcastData.BroadcastName.ToLower()].ActivationEvents?.Invoke();
        }
    }
    public static void BroadcastWithValueActivation(string triggerName, float value)
    {
        ActiveBroadcastItems[triggerName.ToLower()].ActivationWithValueEvents?.Invoke(value);
    }


}
