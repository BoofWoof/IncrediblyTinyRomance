using PixelCrushers.DialogueSystem;
using UnityEngine;
using System.Collections;

public class ConversationManagerScript : MonoBehaviour
{
    public static ConversationManagerScript instance;

    public static bool ConversationOngoing = false;
    public static bool isMacroConvo = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Lua.RegisterFunction("QueueDialogue", null, SymbolExtensions.GetMethodInfo(() => MessageQueue.addDialogue("")));
        Lua.RegisterFunction("QueueWaitDialogue", null, SymbolExtensions.GetMethodInfo(() => MessageQueue.addDialogue("", 0)));

        MessageQueue.addDialogue("Introduction Milo");
        StartCoroutine(WaitForNextConversation());

        transform.parent = DialogueManager.instance.transform;
    }

    public void StartDialogue(string newConversation)
    {
        Conversation newConv = DialogueManager.masterDatabase.GetConversation(newConversation);
        isMacroConvo = Field.LookupBool(newConv.fields, "IsMacro");

        ConversationOngoing = true;

        DialogueManager.StopAllConversations();
        DialogueManager.StartConversation(newConversation);

        if (isMacroConvo)
        {
            PrayerScript.StoryMode = true;
            MusicSelectorScript.SetOverworldSong(3);
        }
        else
        {
            PrayerScript.StoryMode = false;
            ContactsScript.instance.messengerApp.NotificationPing();
        }
    }
    public void OnConversationEnd(Transform actor)
    {
        PrayerScript.StoryMode = false;
        ConversationOngoing = false;
    }

    public IEnumerator WaitForNextConversation()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            if (MessageQueue.GetQueueLength() > 0)
            {
                Dialogue nextDialogue = MessageQueue.getNextDialogue();
                yield return new WaitForSeconds((float)nextDialogue.wait);
                StartDialogue(nextDialogue.dialouge);
            }
        }
    }

}
