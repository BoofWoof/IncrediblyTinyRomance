using DS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ContactsScript : MonoBehaviour
{
    public GameObject MessagingApp;
    private MessengerApp messengerApp;

    private DSDialogue dialogue;

    public CharacterInfo activeCharacter = null;
    public CharacterInfo speakingCharacter = null;

    public Coroutine DialogueCoroutine = null;

    public float SeparationDistance = 400f;
    private List<CharacterInfo> ContactsFound = new List<CharacterInfo>();
    public GameObject ContactButtonPrefab;
    public GameObject ContactListCenter;
    private List<GameObject> ContactList = new List<GameObject>();

    private static bool QuickMessaging = false;

    public void Start()
    {
        StartCoroutine(WaitForNextMessage());
        messengerApp = MessagingApp.GetComponent<MessengerApp>();
        GetComponent<AppScript>().OnShowApp += DeselectCharacter;
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
        for (int idx = 0; idx < ContactsFound.Count; idx++)
        {
            CharacterInfo characterInfo = ContactsFound[idx];

            GameObject newContactButton = Instantiate(ContactButtonPrefab);
            newContactButton.transform.parent = ContactListCenter.transform;
            newContactButton.transform.localRotation = Quaternion.identity;
            newContactButton.transform.localScale = Vector3.one;

            newContactButton.transform.localPosition = Vector3.right * (SeparationDistance * idx - 0.5f * SeparationDistance * (ContactsFound.Count - 1));

            newContactButton.transform.GetChild(0).GetComponent<Image>().sprite = characterInfo.defaultCharacterSprite;

            EventTrigger eventTrigger = newContactButton.AddComponent<EventTrigger>();

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
    public void CheckContacts(CharacterInfo contact)
    {
        if (contact == null || ContactsFound.Contains(contact)) return;
        ContactsFound.Add(contact);
        DeleteContactButtons();
        MakeContactButtons();
    }

    public void SwapToCharacterMessanger(CharacterInfo selectCharacter)
    {
        activeCharacter = selectCharacter;
        GetComponent<AppScript>().Swap(MessagingApp.GetComponent<AppScript>());

        messengerApp.CurrentCharacter = selectCharacter;
        messengerApp.RecreateFromText();
        if (activeCharacter == speakingCharacter && messengerApp.WaitingForChoice)
        {
            DialogueCoroutine = StartCoroutine(messengerApp.RevealOptions(dialogue, activeCharacter));
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

    public IEnumerator MessageProgression()
    {
        while (!dialogue.isDone())
        {
            DSReturnValueInfo returnValue = dialogue.getReturnValue();
            if (returnValue.ReturnValueObject != null)
            {
                if (returnValue.TypeUuid == returnValue.ReturnValueObject.StateUuids[0])
                {
                    yield return new WaitForSeconds(returnValue.ReturnValue);
                }
            }
            if (dialogue.isSingleOption())
            {
                yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages * dialogue.getText().Length/100f);
                messengerApp.NotificationPing();
                string message_text = dialogue.getText();
                CharacterInfo newSpeakingCharacter = dialogue.getCharacter();
                if (newSpeakingCharacter != null)
                {
                    speakingCharacter = newSpeakingCharacter;
                }
                if (activeCharacter == speakingCharacter)
                {
                    messengerApp.RecreateFromText();

                    MessageBoxScript message_info = messengerApp.MakeLeftMessage(message_text);

                    messengerApp.UpdateTextHistory(speakingCharacter, "<a>" + message_text + "\n");

                    if(MessagingVariables.TimeBetweenMessages > 0)
                    {
                        DialogueCoroutine = StartCoroutine(message_info.CharacterProgression(message_text));
                    } else
                    {
                        message_info.InstantComplete(message_text);
                    }
                    yield return DialogueCoroutine;
                }
                else
                {
                    messengerApp.UpdateTextHistory(speakingCharacter, "<a>" + message_text + "\n");
                }
                yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages);
                dialogue.setChoice("Next Dialogue");
                continue;
            }
            else if (dialogue.isMultipleOptions())
            {
                yield return new WaitForSeconds(MessagingVariables.TimeBetweenMessages * dialogue.getText().Length / 100f);
                messengerApp.SetTextChoices(dialogue.getChoices());
                if (activeCharacter == speakingCharacter)
                {
                    DialogueCoroutine = StartCoroutine(messengerApp.RevealOptions(dialogue, activeCharacter));
                }

                yield return StartCoroutine(messengerApp.WaitForChoiceSelection());
                continue;
            }
            else
            {
                break;
            }
        }
        messengerApp.MakeDivisionBar();
        messengerApp.UpdateTextHistory(speakingCharacter, "<c>" + "\n");
        StartCoroutine(WaitForNextMessage());
    }
    public IEnumerator WaitForNextMessage()
    {
        do
        {
            yield return new WaitForSeconds(2f);
        } while (MessageQueue.GetQueueLength() == 0);

        dialogue = MessageQueue.getNextDialogue();
        CheckContacts(dialogue.getCharacter());

        StartCoroutine(MessageProgression());
    }
}
