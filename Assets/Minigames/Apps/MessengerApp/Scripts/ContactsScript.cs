using DS;
using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContactsScript : MonoBehaviour
{
    public static ContactsScript instance;
    public MessengerApp messengerApp;

    public PixelCrushers.DialogueSystem.CharacterInfo activeCharacter = null;

    public Coroutine DialogueCoroutine = null;

    public Transform[] ContactPositions;

    public float SeparationDistance = 400f;
    private Dictionary<int, PixelCrushers.DialogueSystem.CharacterInfo> ContactsFound = new Dictionary<int, PixelCrushers.DialogueSystem.CharacterInfo>();
    public GameObject ContactButtonPrefab;
    public GameObject ContactListCenter;
    private List<GameObject> ContactList = new List<GameObject>();

    public bool ConversationIsActive = false;

    public int speakingCharacterId = -1;
    private PixelCrushers.DialogueSystem.CharacterInfo tempSpeakingCharacter;

    public void Start()
    {
        instance = this;
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
        tempSpeakingCharacter = subtitle.speakerInfo;
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

        //Add contact if this is their first message.
        CheckContacts(tempSpeakingCharacter);
    }

    public IEnumerator SendSingleMessage(PixelCrushers.DialogueSystem.CharacterInfo newSpeaker, string message_text)
    {
        yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages + MessagingVariables.TimePerCharacter * message_text.Length);
        messengerApp.AddLeftMessage(newSpeaker.id, message_text);        
        (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
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
        yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages);
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
        foreach (PixelCrushers.DialogueSystem.CharacterInfo characterInfo in ContactsFound.Values)
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
    public void CheckContacts(PixelCrushers.DialogueSystem.CharacterInfo contact)
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

    public void SwapToCharacterMessanger(PixelCrushers.DialogueSystem.CharacterInfo selectCharacter)
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
}
