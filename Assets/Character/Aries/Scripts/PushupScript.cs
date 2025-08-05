using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct PushupEvent
{
    public int PushupCount;
    public string Character;
    public VoiceLineSO VoiceLine; 
}

public class PushupScript : MonoBehaviour
{
    public int PushupCount = 0;
    public List<PushupEvent> PushupEvents = new List<PushupEvent>();

    // Update is called once per frame
    public void StartPushup()
    {
        PushupCount = 0;
    }
    public void Pushup()
    {
        PushupCount++;
        Debug.Log("Trigger Pushup: " + PushupCount.ToString());

        foreach (PushupEvent e in PushupEvents)
        {
            if (e.PushupCount != PushupCount) continue;

            string name = "MacroAries";
            if (e.Character.Length > 0) name =e.Character;

            foreach (CharacterSpeechScript c in CharacterSpeechScript.CharacterSpeechInstances)
            {
                c.PlaySpeech(name, e.VoiceLine);
            }
        }
    }
}
