using DS;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace DS
{
    public class MessengerApp : AppScript
    {
        [Header("Objects")]
        public GameObject message_box;
        public GameObject message_options;
        public RectTransform content_rect;

        [Header("Data")]
        private string MessageHistory = "";
        public MessageBoxScript LastMessage = null;
        public bool LastLeft = false;
        public Sprite CurrentCharacterSprite;

        [Header("Tuning")]
        public float message_buffer = 25;
        public float start_buffer = 50;
        public float end_buffer = 25;
        public float left_buffer = 200;
        public float right_buffer = 200;
        public float default_time_between_message = 1.0f;

        private float conversation_height;
        private DSDialogue dialogue;

        [Header("NotificationSounds")]
        public AudioSource notification_source;
        public AudioClip new_message_notification;

        // Start is called before the first frame update
        void Awake()
        {
            conversation_height = start_buffer;

            StartCoroutine(WaitForNextMessage());

            Hide(true);
            RegisterInputActions();
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
                    RecreateFromText();

                    notification_source.clip = new_message_notification;
                    notification_source.Play();

                    Sprite newSprite = dialogue.getSprite();
                    if (newSprite != null)
                    {
                        CurrentCharacterSprite = newSprite;
                    }

                    string message_text = dialogue.getText();
                    MessageBoxScript message_info = MakeLeftMessage(message_text);

                    yield return StartCoroutine(message_info.CharacterProgression(message_text));
                    yield return new WaitForSeconds(default_time_between_message);
                    MessageHistory += "<a>" + message_text + "\n";
                    dialogue.setChoice("Next Dialogue");
                    continue;
                }
                else if (dialogue.isMultipleOptions())
                {
                    //MOVE THIS TO AN ACTUAL WORKING LOCATION
                    GameObject option_message = Instantiate(message_options, content_rect);
                    option_message.transform.localPosition = new Vector2(content_rect.rect.width - right_buffer, -conversation_height);
                    option_message.transform.localScale = Vector3.one;
                    option_message.transform.localRotation = Quaternion.identity;
                    MessageOptionsScript messageOptionScript = option_message.GetComponent<MessageOptionsScript>();
                    messageOptionScript.options = dialogue.getChoices();
                    messageOptionScript.CreateButtons();
                    conversation_height += messageOptionScript.height + message_buffer;
                    content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, conversation_height + end_buffer);
                    GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;

                    yield return StartCoroutine(messageOptionScript.WaitForResponse());
                    dialogue.setChoice(messageOptionScript.message);
                    MessageHistory += "<b>" + messageOptionScript.message + "\n";

                    LastLeft = false;
                    continue;
                }
                else
                {
                    break;
                }
            }
            StartCoroutine(WaitForNextMessage());
        }

        private MessageBoxScript MakeRightMessage(string message_text)
        {
            GameObject new_message = Instantiate(message_box, Vector2.zero, Quaternion.identity);

            MessageBoxScript message_info = new_message.GetComponent<MessageBoxScript>();

            message_info.SwitchLeft();

            new_message.transform.parent = content_rect;
            new_message.transform.localPosition = new Vector2(content_rect.rect.width - right_buffer, -conversation_height);
            new_message.transform.localScale = Vector3.one;
            new_message.transform.localRotation = Quaternion.identity;

            conversation_height += message_info.GetFinalHeight(message_text) + message_buffer;
            content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, conversation_height + end_buffer);
            GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;

            LastLeft = false;
            return message_info;
        }

        private MessageBoxScript MakeLeftMessage(string message_text)
        {
            GameObject new_message = Instantiate(message_box, Vector2.zero, Quaternion.identity);

            MessageBoxScript message_info = new_message.GetComponent<MessageBoxScript>();

            if (LastLeft && LastMessage != null)
            {
                conversation_height -= LastMessage.stem_height;
                LastMessage.UseSecondaryBacking();
                LastMessage.UpdateBackingSize();
            }

            new_message.transform.parent = content_rect;
            new_message.transform.localScale = Vector3.one;
            new_message.transform.localPosition = new Vector2(left_buffer, -conversation_height);
            new_message.transform.localRotation = Quaternion.identity;

            message_info.SetSprite(CurrentCharacterSprite);
            conversation_height += message_info.GetFinalHeight(message_text) + message_buffer;
            content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, conversation_height + end_buffer);
            GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;

            LastLeft = true;
            LastMessage = message_info;
            return message_info;
        }

        public void RecreateFromText()
        {
            ResetMessanger();
            RecreateMessages();
        }

        private void RecreateMessages()
        {
            string[] messageHistory = MessageHistory.Split("\n");

            foreach (string message in messageHistory)
            {
                string pattern = @"(?<=\<).*?(?=\>)";
                string message_source = Regex.Match(message, pattern).Value;
                if (message_source == "a")
                {
                    string trimmed_message = message.Substring(3);
                    MessageBoxScript message_info = MakeLeftMessage(trimmed_message);
                    message_info.InstantComplete(trimmed_message);
                } else if (message_source == "b")
                {
                    string trimmed_message = message.Substring(3);
                    MessageBoxScript message_info = MakeRightMessage(trimmed_message);
                    message_info.InstantComplete(trimmed_message);
                }
            }
        }

        public void ResetMessanger()
        {
            LastMessage = null;
            foreach (Transform child in content_rect.transform)
            {
                // Destroy the child GameObject
                Destroy(child.gameObject);
            }
            conversation_height = start_buffer;
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
}
