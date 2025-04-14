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
    public GameObject MessagingApp;
    private MessengerApp messengerApp;

    public PixelCrushers.DialogueSystem.CharacterInfo activeCharacter = null;
    public PixelCrushers.DialogueSystem.CharacterInfo speakingCharacter = null;

    public Coroutine DialogueCoroutine = null;

    public float SeparationDistance = 400f;
    private Dictionary<int, PixelCrushers.DialogueSystem.CharacterInfo> ContactsFound = new Dictionary<int, PixelCrushers.DialogueSystem.CharacterInfo>();
    public GameObject ContactButtonPrefab;
    public GameObject ContactListCenter;
    private List<GameObject> ContactList = new List<GameObject>();

    private static bool QuickMessaging = false;

    public void Start()
    {
        //StartCoroutine(WaitForNextMessage());
        messengerApp = MessagingApp.GetComponent<MessengerApp>();
        GetComponent<AppScript>().OnShowApp += DeselectCharacter;

        DialogueManager.StartConversation("Test Conversation");
    }

    public void OnConversationLine(Subtitle subtitle)
    {
        PixelCrushers.DialogueSystem.CharacterInfo speakingCharacter = subtitle.speakerInfo;
        if (speakingCharacter.Name == "Player") return;

        string messageText = string.Format(subtitle.formattedText.text);

        Debug.Log("New message: " + messageText);

        CheckContacts(speakingCharacter);

        string voiceFilePath = subtitle.dialogueEntry.fields.Find(f => f.title == "VoiceLinesSO").value;
        if (voiceFilePath.Length < 5)
        {
            StartCoroutine(SendSingleMessage(speakingCharacter, messageText));
        } else
        {
            StartCoroutine(SendAudioMessage(speakingCharacter, voiceFilePath));
        }
    }

    public IEnumerator SendAudioMessage(PixelCrushers.DialogueSystem.CharacterInfo newSpeaker, string voiceFilePath)
    {
        yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages);
        messengerApp.NotificationPing();
        if (newSpeaker != null)
        {
            speakingCharacter = newSpeaker;
        }
        if (activeCharacter != null && activeCharacter.id == speakingCharacter.id)
        {
            messengerApp.RecreateFromText();

            messengerApp.MakeAudioMessage(voiceFilePath);

            messengerApp.UpdateTextHistory(speakingCharacter, "<v>" + voiceFilePath + "\n");
        }
        else
        {
            messengerApp.UpdateTextHistory(speakingCharacter, "<v>" + voiceFilePath + "\n");
        }

        (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
    }

    public IEnumerator SendSingleMessage(PixelCrushers.DialogueSystem.CharacterInfo newSpeaker, string message_text)
    {
        yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages * message_text.Length / 100f);
        messengerApp.NotificationPing();
        if (newSpeaker != null)
        {
            speakingCharacter = newSpeaker;
        }
        if (activeCharacter != null && activeCharacter.id == speakingCharacter.id)
        {
            messengerApp.RecreateFromText();

            MessageBoxScript message_info = messengerApp.MakeLeftMessage(message_text);

            messengerApp.UpdateTextHistory(speakingCharacter, "<a>" + message_text + "\n");

            if (MessagingVariables.TimeBetweenMessages > 0)
            {
                DialogueCoroutine = StartCoroutine(message_info.CharacterProgression(message_text));
            }
            else
            {
                message_info.InstantComplete(message_text);
            }
            yield return DialogueCoroutine;
        }
        else
        {
            messengerApp.UpdateTextHistory(speakingCharacter, "<a>" + message_text + "\n");
        }
        (DialogueManager.dialogueUI as AbstractDialogueUI).OnContinueConversation();
    }

    public void OnConversationResponseMenu(Response[] responses)
    {
        Debug.Log("Creating choices: " + responses.Length.ToString());
        StartCoroutine(SendChoicesMessage(responses));
    }

    public IEnumerator SendChoicesMessage(Response[] responses)
    {
        yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages);
        messengerApp.SetTextChoices(responses);
        if (activeCharacter != null && activeCharacter.id == speakingCharacter.id)
        {
            DialogueCoroutine = StartCoroutine(messengerApp.RevealOptions(activeCharacter));
        }
    }

    public void OnConversationEnd(Transform actor)
    {
        messengerApp.MakeDivisionBar();
        messengerApp.UpdateTextHistory(speakingCharacter, "<c>" + "\n");
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
            newContactButton.transform.parent = ContactListCenter.transform;
            newContactButton.transform.localRotation = Quaternion.identity;
            newContactButton.transform.localScale = Vector3.one;

            newContactButton.transform.localPosition = Vector3.right * (SeparationDistance * idx - 0.5f * SeparationDistance * (ContactsFound.Count - 1));

            newContactButton.transform.GetChild(0).GetComponent<Image>().sprite = characterInfo.portrait;

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
        DeleteContactButtons();
        MakeContactButtons();
    }

    public void SwapToCharacterMessanger(PixelCrushers.DialogueSystem.CharacterInfo selectCharacter)
    {
        activeCharacter = selectCharacter;
        GetComponent<AppScript>().Swap(MessagingApp.GetComponent<AppScript>());

        messengerApp.CurrentCharacter = selectCharacter;
        messengerApp.RecreateFromText();
        if (activeCharacter != null && activeCharacter.id == speakingCharacter.id && messengerApp.WaitingForChoice)
        {
            DialogueCoroutine = StartCoroutine(messengerApp.RevealOptions(activeCharacter));
        }
    }

    public void DeselectCharacter()
    {
        activeCharacter = null;
        if (DialogueCoroutine != null)
        {
            StopCoroutine(DialogueCoroutine);
            DialogueCoroutine = null;
        }
    }
}
