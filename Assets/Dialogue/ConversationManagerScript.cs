using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConversationManagerScript : MonoBehaviour
{
    public static ConversationManagerScript instance;

    public static bool ConversationOngoing = false;
    public static bool isMacroConvo = false;
    public static bool WaitingForEvent = false;

    public static List<string> BannedDialogues = new List<string>();

    private void Awake()
    {
        instance = this;
    }

    public void ForceNextDialogue()
    {
        (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
    }

    void Start()
    {
        Lua.RegisterFunction("QueueDialogue", null, SymbolExtensions.GetMethodInfo(() => MessageQueue.addDialogue("")));
        Lua.RegisterFunction("QueueWaitDialogue", null, SymbolExtensions.GetMethodInfo(() => MessageQueue.addDialogue("", 0)));

        StartCoroutine(WaitForNextConversation());

        transform.parent = DialogueManager.instance.transform;
    }

    public void StartDialogue(string newConversation)
    {

        Conversation newConv = DialogueManager.masterDatabase.GetConversation(newConversation);
        bool allowRepeat = Field.LookupBool(newConv.fields, "AllowRepeat");
        if(!allowRepeat) BannedDialogues.Add(newConversation);

        isMacroConvo = Field.LookupBool(newConv.fields, "IsMacro");

        ConversationOngoing = true;

        DialogueManager.StopAllConversations();
        DialogueManager.StartConversation(newConversation);

        if (isMacroConvo)
        {
            PrayerScript.StoryMode = true;
            //MusicSelectorScript.SetOverworldSong(3);
        }
        else
        {
            PrayerScript.StoryMode = false;
        }
    }
    public void OnConversationEnd(Transform actor)
    {
        PrayerScript.StoryMode = false;
        ConversationOngoing = false;
        WaitingForEvent = false;
    }

    public IEnumerator WaitForNextConversation()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (GameStateMonitor.isEventActive()) continue;
            if (!ConversationOngoing && MessageQueue.GetQueueLength() > 0)
            {
                Dialogue nextDialogue = MessageQueue.getNextDialogue();

                if (BannedDialogues.Contains(nextDialogue.dialouge)) continue;

                yield return new WaitForSeconds((float)nextDialogue.wait);
                StartDialogue(nextDialogue.dialouge);
            }
        }
    }

}
