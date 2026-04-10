using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PingData
{
    public string PingKey;
    public AudioClip PingSound;
    public Texture2D PingTexture;
}

[CreateAssetMenu(fileName = "PingListSO", menuName = "Messanger/PingListSO")]
public class PingListSO : ScriptableObject
{
    public PingData DefaultPing;
    public List<PingData> PingOptions;

    public bool ContainsKey(string text)
    {
        foreach (PingData pData in PingOptions)
        {
            if (text.ToLower().Contains($"<{pData.PingKey.ToLower()}>"))
            {
                return true;
            }
        }

        return false;
    }
}
