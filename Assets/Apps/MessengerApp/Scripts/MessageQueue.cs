using DS;
using System.Collections.Generic;
using UnityEngine;

public class MessageQueue
{
    private static List<DSDialogue> dialogueQueue = new List<DSDialogue>();

    public static int GetQueueLength()
    {
        return dialogueQueue.Count;
    } 

    public static DSDialogue getNextDialogue()
    {
        if(dialogueQueue.Count == 0) { return null; }

        DSDialogue nextDialogue = dialogueQueue[0];
        dialogueQueue.Remove(nextDialogue);
        return nextDialogue;
    }

    public static void addDialogue(DSDialogue newDialogue)
    {
        dialogueQueue.Add(newDialogue);
    }
}
