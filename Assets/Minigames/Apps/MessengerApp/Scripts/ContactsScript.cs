using DS;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocalCharacterInfo
{
    public Sprite portrait;
    public string Name;
    public int id;

    public LocalCharacterInfo FromPCCharacterInfo(PixelCrushers.DialogueSystem.CharacterInfo characterInfo)
    {
        portrait = characterInfo.portrait;
        Name = characterInfo.Name;
        id = characterInfo.id;

        return this;
    }

    public LocalCharacterInfo FromName(string characterName)
    {
        Name = characterName;

        Actor actor = DialogueManager.masterDatabase.GetActor(characterName);
        portrait = actor.spritePortrait;
        id = actor.id;

        return this;
    }

    public LocalCharacterInfo FromID(int characterID)
    {
        id = characterID;

        Actor actor = DialogueManager.masterDatabase.GetActor(characterID);
        Name = actor.Name;
        portrait = actor.spritePortrait;

        return this;
    }
}

public class ContactsScript : Saver
{
    public static ContactsScript instance;
    public MessengerApp messengerApp;

    public LocalCharacterInfo activeCharacter = null;

    public Coroutine DialogueCoroutine = null;

    public Transform[] ContactPositions;

    public float SeparationDistance = 400f;
    private Dictionary<int, LocalCharacterInfo> ContactsFound = new Dictionary<int, LocalCharacterInfo>();
    public GameObject ContactButtonPrefab;
    public GameObject ContactListCenter;
    private List<GameObject> ContactList = new List<GameObject>();

    public bool ConversationIsActive = false;

    public int speakingCharacterId = -1;
    private LocalCharacterInfo tempSpeakingCharacter;

    override public void Start()
    {
        base.Start();
        instance = this;
    }

    public void SendMessageByName(string speakerName, string messageText)
    {
        LocalCharacterInfo character = new LocalCharacterInfo().FromName(speakerName);

        StartCoroutine(SendSingleMessage(character, messageText, false));
    }

    public void OnConversationLine(Subtitle subtitle)
    {
        if (subtitle.speakerInfo.GetFieldBool("WaitThem"))
        {
            ConversationManagerScript.WaitingForEvent = true;
            return;
        }
        if (subtitle.speakerInfo.GetFieldBool("SkipThem"))
        {
            Debug.Log("Skipping Conversation Step");
            (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
            return;
        }
        if (subtitle.speakerInfo.GetFieldBool("IsRadio")) {
            ConversationIsActive = false;
            return;
        }
        if (subtitle.speakerInfo.GetFieldBool("IsMacro"))
        {
            ConversationIsActive = false;
            return;
        }
        tempSpeakingCharacter = new LocalCharacterInfo().FromPCCharacterInfo(subtitle.speakerInfo);
        if (tempSpeakingCharacter.Name == "Player") return;

        ConversationIsActive = true;

        //End line if speaking character switches.
        if (speakingCharacterId != -1 && speakingCharacterId != tempSpeakingCharacter.id)
        {
            messengerApp.AddDivisionBar(tempSpeakingCharacter.id);
        }
        speakingCharacterId = tempSpeakingCharacter.id;

        string messageText = string.Format(subtitle.formattedText.text);
        StartCoroutine(SendSingleMessage(tempSpeakingCharacter, messageText));
    }

    public IEnumerator SendSingleMessage(LocalCharacterInfo newSpeaker, string message_text, bool continueConversation = true)
    {
        Debug.Log(newSpeaker.Name);
        Debug.Log(newSpeaker.id);
        Debug.Log(newSpeaker.portrait);
        yield return new WaitForSeconds((MessagingVariables.TimeBetweenMessages + MessagingVariables.TimePerCharacter * message_text.Length) / MessagingVariables.SetTimeDivider);
        CheckContacts(newSpeaker);
        messengerApp.AddLeftMessage(newSpeaker.id, message_text);        
        if(continueConversation) (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
    }

    public void OnConversationResponseMenu(Response[] responses)
    {
        if (DialogueManager.CurrentConversationState.subtitle.speakerInfo.GetFieldBool("IsRadio")) return;
        if (DialogueManager.CurrentConversationState.subtitle.speakerInfo.GetFieldBool("IsMacro")) return;
        if (ConversationManagerScript.isMacroConvo) return;
        speakingCharacterId = responses[0].destinationEntry.ConversantID;
        StartCoroutine(SendChoicesMessage(responses));

    }

    public IEnumerator SendChoicesMessage(Response[] responses)
    {
        yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages / MessagingVariables.SetTimeDivider);
        messengerApp.SendOptions(speakingCharacterId, responses);
    }

    public void OnConversationEnd(Transform actor)
    {
        if (!isActiveAndEnabled) return;
        messengerApp.AddDivisionBar(speakingCharacterId);
        speakingCharacterId = -1;
        ConversationIsActive = false;
    }

    public void InstantMessaging()
    {
        MessagingVariables.InstantMessaging();
        Debug.Log("Quick messaging engaged.");
    }
    public void SemiInstantMessaging()
    {
        MessagingVariables.SemiInstantMessaging();
        Debug.Log("Semi-Instant messaging engaged.");
    }
    public void DefaultMessaging()
    {
        MessagingVariables.DefaultMessaging();
        Debug.Log("Slow messaging engaged.");
    }

    public void MakeContactButtons()
    {
        int idx = 0;
        foreach (LocalCharacterInfo characterInfo in ContactsFound.Values)
        {
            GameObject newContactButton = Instantiate(ContactButtonPrefab);

            newContactButton.transform.parent = ContactPositions[idx].parent;
            newContactButton.transform.localPosition = ContactPositions[idx].localPosition;
            newContactButton.transform.localRotation = Quaternion.identity;
            newContactButton.transform.localScale = Vector3.one;

            newContactButton.transform.GetChild(0).GetComponent<Image>().sprite = characterInfo.portrait;
            if (messengerApp.UncheckMessages.Contains(characterInfo.id))
            {
                newContactButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            } else
            {
                newContactButton.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            }

            EventTrigger eventTrigger = newContactButton.transform.GetChild(0).GetChild(0).gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            entry.callback.AddListener((eventData) => {
                PointerEventData pointerData = (PointerEventData)eventData;
                if (pointerData.button != PointerEventData.InputButton.Left) return;
                SwapToCharacterMessanger(characterInfo);
            });
            eventTrigger.triggers.Add(entry);

            ContactList.Add(newContactButton);

            idx++;
        }
    }
    public void DeleteContactButtons()
    {
        foreach (GameObject obj in ContactList)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        ContactList.Clear();
    }
    public void CheckContacts(LocalCharacterInfo contact)
    {
        if (contact == null || ContactsFound.Keys.Contains(contact.id)) return;

        ContactsFound.Add(contact.id, contact);
        RebuildContacts();
    }

    public void RebuildContacts()
    {
        DeleteContactButtons();
        MakeContactButtons();
    }

    public void SwapToCharacterMessanger(LocalCharacterInfo selectCharacter)
    {
        activeCharacter = selectCharacter;
        messengerApp.SetCharacter(selectCharacter);
        GetComponent<AppScript>().Swap(messengerApp);
    }

    public void DeselectCharacter()
    {
        activeCharacter = null;
        RebuildContacts();
    }

    [Serializable]
    public class ConversationSave
    {
        public List<string> MessageHistorys = new List<string>();
        public List<int> MessageIDs = new List<int>();
        public List<int> ContactsFoundID = new List<int>();
        public void SetMessageHistory(Dictionary<int, string> messageHistory)
        {
            MessageHistorys = messageHistory.Values.ToList();
            MessageIDs = messageHistory.Keys.ToList();
        }
        public void SetContactsFound(Dictionary<int, LocalCharacterInfo> contactsFound)
        {
            ContactsFoundID = contactsFound.Keys.ToList();
        }
        public Dictionary<int, string> GetMessageHistory()
        {
            Dictionary<int, string> messageHistory = new Dictionary<int, string>();
            for (int i = 0; i < MessageIDs.Count; i++)
            {
                messageHistory[MessageIDs[i]] = MessageHistorys[i];
            }
            return messageHistory;
        }
        public Dictionary<int, LocalCharacterInfo> GetContactsFound()
        {
            Dictionary<int, LocalCharacterInfo> contactsFound = new Dictionary<int, LocalCharacterInfo>();
            for (int i = 0; i < ContactsFoundID.Count; i++)
            {
                contactsFound[ContactsFoundID[i]] = new LocalCharacterInfo().FromID(ContactsFoundID[i]);
            }
            return contactsFound;
        }
    }

    public override string RecordData()
    {
        ConversationSave newSaveData = new ConversationSave();

        newSaveData.SetMessageHistory(messengerApp.MessageHistorys);
        newSaveData.SetContactsFound(ContactsFound);

        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        ConversationSave saveData = SaveSystem.Deserialize<ConversationSave>(s);
        ContactsFound = saveData.GetContactsFound();
        messengerApp.MessageHistorys = saveData.GetMessageHistory();
        RebuildContacts();
    }
}
