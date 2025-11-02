using DS;
using System.Collections.Generic;
using UnityEngine;


public struct Dialogue
{
    public string dialouge;
    public double wait;

    public Dialogue(string dialogue, double wait)
    {
        this.dialouge = dialogue;
        this.wait = wait;
    }
}
public class MessageQueue
{
    private static List<Dialogue> dialogueQueue = new List<Dialogue>();

    public static int GetQueueLength()
    {
        return dialogueQueue.Count;
    } 

    public static Dialogue getNextDialogue()
    {
        if(dialogueQueue.Count == 0) { return new Dialogue(null, 0); }

        Dialogue nextDialogue = dialogueQueue[0];
        dialogueQueue.Remove(nextDialogue);
        return nextDialogue;
    }

    public static void addDialogue(string newDialogue)
    {
        dialogueQueue.Add(new Dialogue(newDialogue, 0));
        Debug.Log(dialogueQueue.Count);
    }
    public static void addDialogue(string newDialogue, double wait)
    {
        dialogueQueue.Add(new Dialogue(newDialogue, wait));
    }
}
