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
    public GameObject MessagingApp;
    public MessengerApp messengerApp;

    public PixelCrushers.DialogueSystem.CharacterInfo activeCharacter = null;
    public PixelCrushers.DialogueSystem.CharacterInfo speakingCharacter = null;

    public Coroutine DialogueCoroutine = null;

    public Transform[] ContactPositions;

    public float SeparationDistance = 400f;
    private Dictionary<int, PixelCrushers.DialogueSystem.CharacterInfo> ContactsFound = new Dictionary<int, PixelCrushers.DialogueSystem.CharacterInfo>();
    public GameObject ContactButtonPrefab;
    public GameObject ContactListCenter;
    private List<GameObject> ContactList = new List<GameObject>();

    public List<int> UncheckedMessages;

    private static bool QuickMessaging = false;

    public void Start()
    {
        instance = this;

        messengerApp = MessagingApp.GetComponent<MessengerApp>();
        GetComponent<AppScript>().OnShowApp += DeselectCharacter;

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
        if (subtitle.speakerInfo.GetFieldBool("IsRadio")) return;
        if (subtitle.speakerInfo.GetFieldBool("IsMacro")) return;
        PixelCrushers.DialogueSystem.CharacterInfo tempSpeakingCharacter = subtitle.speakerInfo;
        if (tempSpeakingCharacter.Name == "Player") return;
        if (speakingCharacter != null && speakingCharacter.id != tempSpeakingCharacter.id) {
            AddEndBar();
        }

        if(activeCharacter == null || (activeCharacter.id != tempSpeakingCharacter.id))
        {
            HudScript.ShowMessageNotification(true);
            if (!UncheckedMessages.Contains(tempSpeakingCharacter.id))
            {
                UncheckedMessages.Add(tempSpeakingCharacter.id);
            }
        }

        string messageText = string.Format(subtitle.formattedText.text);

        Debug.Log("New message: " + messageText);

        CheckContacts(tempSpeakingCharacter);

        string voiceFilePath = subtitle.dialogueEntry.fields.Find(f => f.title == "VoiceLinesSO").value;
        if (voiceFilePath.Length < 5)
        {
            StartCoroutine(SendSingleMessage(tempSpeakingCharacter, messageText));
        } else
        {
            StartCoroutine(SendAudioMessage(tempSpeakingCharacter, voiceFilePath));
        }

        RebuildContacts();
    }

    public IEnumerator SendAudioMessage(PixelCrushers.DialogueSystem.CharacterInfo newSpeaker, string voiceFilePath)
    {
        yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages);
        if (newSpeaker != null)
        {
            speakingCharacter = newSpeaker;
        }
        if (activeCharacter != null && activeCharacter.id == speakingCharacter.id)
        {
            messengerApp.NotificationPing();
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
        if (newSpeaker != null)
        {
            speakingCharacter = newSpeaker;
        }
        messengerApp.NotificationPing();
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
        speakingCharacter = DialogueManager.CurrentConversationState.subtitle.speakerInfo;
        if (DialogueManager.CurrentConversationState.subtitle.speakerInfo.GetFieldBool("IsRadio")) return;
        if (DialogueManager.CurrentConversationState.subtitle.speakerInfo.GetFieldBool("IsMacro")) return;
        if (ConversationManagerScript.isMacroConvo) return;
        Debug.Log(speakingCharacter.nameInDatabase);
        Debug.Log("Creating choices: " + responses.Length.ToString());
        StartCoroutine(SendChoicesMessage(responses));

        RebuildContacts();
    }

    public IEnumerator SendChoicesMessage(Response[] responses)
    {
        yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages);
        messengerApp.SetTextChoices(responses);
        if (activeCharacter != null && activeCharacter.id == speakingCharacter.id)
        {
            DialogueCoroutine = StartCoroutine(messengerApp.RevealOptions(activeCharacter));
        } else
        {
            if (messengerApp.WaitingForChoice) HudScript.ShowMessageNotification(true);
        }
    }

    public void OnConversationEnd(Transform actor)
    {
        if (ConversationManagerScript.isMacroConvo) return;
        AddEndBar();
        speakingCharacter = null;
    }

    public void AddEndBar()
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


            newContactButton.transform.parent = ContactPositions[idx].parent;
            newContactButton.transform.localPosition = ContactPositions[idx].localPosition;
            newContactButton.transform.localRotation = Quaternion.identity;
            newContactButton.transform.localScale = Vector3.one;

            newContactButton.transform.GetChild(0).GetComponent<Image>().sprite = characterInfo.portrait;
            if (UncheckedMessages.Contains(characterInfo.id))
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

    public static void CheckShowMessageNotification()
    {
        HudScript.ShowMessageNotification(instance.UncheckedMessages.Count > 0);
    }

    public void SwapToCharacterMessanger(PixelCrushers.DialogueSystem.CharacterInfo selectCharacter)
    {
        if (DialogueCoroutine != null)
        {
            StopCoroutine(DialogueCoroutine);
            DialogueCoroutine = null;
        }

        activeCharacter = selectCharacter;
        GetComponent<AppScript>().Swap(MessagingApp.GetComponent<AppScript>());

        if (UncheckedMessages.Contains(activeCharacter.id))
        {
            UncheckedMessages.Remove(activeCharacter.id);
        }
        CheckShowMessageNotification();

        messengerApp.CurrentCharacter = selectCharacter;
        messengerApp.RecreateFromText();
        if (activeCharacter != null && speakingCharacter != null && activeCharacter.id == speakingCharacter.id && messengerApp.WaitingForChoice)
        {
            DialogueCoroutine = StartCoroutine(messengerApp.RevealOptions(activeCharacter));
        }
    }

    public void DeselectCharacter()
    {
        activeCharacter = null;
        CheckShowMessageNotification();
        if (messengerApp.WaitingForChoice) HudScript.ShowMessageNotification(true);

        RebuildContacts();
    }
}
