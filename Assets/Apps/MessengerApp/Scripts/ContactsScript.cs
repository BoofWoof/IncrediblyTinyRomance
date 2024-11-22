using DS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactsScript : MonoBehaviour
{
    public GameObject MessagingApp;
    private MessengerApp messengerApp;

    private DSDialogue dialogue;
    public float default_time_between_message = 1.0f;

    public CharacterInfo activeCharacter = null;
    public CharacterInfo speakingCharacter = null;

    public Coroutine ChoiceCoroutine = null;

    public void Start()
    {
        StartCoroutine(WaitForNextMessage());
        messengerApp = MessagingApp.GetComponent<MessengerApp>();
        GetComponent<AppScript>().OnShowApp += DeselectCharacter;
    }

    public void SwapToCharacterMessanger(CharacterInfo selectCharacter)
    {
        activeCharacter = selectCharacter;
        GetComponent<AppScript>().Swap(MessagingApp.GetComponent<AppScript>());

        messengerApp.CurrentCharacter = selectCharacter;
        messengerApp.RecreateFromText();
        if (activeCharacter == speakingCharacter)
        {
            ChoiceCoroutine = StartCoroutine(messengerApp.RevealOptions(dialogue, activeCharacter));
        }
    }

    public void DeselectCharacter()
    {
        activeCharacter = null;
        if (ChoiceCoroutine != null)
        {
            StopCoroutine(ChoiceCoroutine);
            ChoiceCoroutine = null;
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
                continue;
            }
            yield return new WaitForSeconds(default_time_between_message);
            if (dialogue.isSingleOption())
            {
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

                    yield return StartCoroutine(message_info.CharacterProgression(message_text));
                }
                else
                {
                    messengerApp.UpdateTextHistory(speakingCharacter, "<a>" + message_text + "\n");
                }
                yield return new WaitForSeconds(default_time_between_message);
                dialogue.setChoice("Next Dialogue");
                continue;
            }
            else if (dialogue.isMultipleOptions())
            {
                messengerApp.SetTextChoices(dialogue.getChoices());
                if (activeCharacter == speakingCharacter)
                {
                    ChoiceCoroutine = StartCoroutine(messengerApp.RevealOptions(dialogue, activeCharacter));
                }

                yield return StartCoroutine(messengerApp.WaitForChoiceSelection());
                continue;
            }
            else
            {
                break;
            }
        }
        StartCoroutine(WaitForNextMessage());
    }
    public IEnumerator WaitForNextMessage()
    {
        do
        {
            yield return new WaitForSeconds(10f);
        } while (MessageQueue.GetQueueLength() == 0);

        dialogue = MessageQueue.getNextDialogue();

        StartCoroutine(MessageProgression());
    }
}
