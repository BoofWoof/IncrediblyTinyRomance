using DS;
using DS.Data;
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
        public GameObject chat_break;

        [Header("Data")]
        private Dictionary<CharacterInfo, string> MessageHistorys = new Dictionary<CharacterInfo, string>();
        public MessageBoxScript LastMessage = null;
        public bool LastLeft = false;

        public CharacterInfo CurrentCharacter;

        public List<string> Choices;
        public bool WaitingForChoice = false;

        [Header("Tuning")]
        public float message_buffer = 25;
        public float start_buffer = 50;
        public float end_buffer = 25;
        public float left_buffer = 200;
        public float right_buffer = 200;

        private float conversation_height;

        [Header("NotificationSounds")]
        public AudioSource notification_source;
        public AudioClip new_message_notification;

        // Start is called before the first frame update
        void Awake()
        {
            conversation_height = start_buffer;

            Hide(true);
            RegisterInputActions();
        }

        public void SetTextChoices(List<string> choices)
        {
            WaitingForChoice = true;
            Choices = choices;
            LastLeft = false;
        }

        public void MakeDivisionBar()
        {
            conversation_height += 50;
            GameObject breakBar = Instantiate(chat_break, Vector2.zero, Quaternion.identity);

            breakBar.transform.parent = content_rect;
            breakBar.transform.localScale = Vector3.one;
            breakBar.transform.localRotation = Quaternion.identity;
            breakBar.transform.localPosition = new Vector2(content_rect.rect.width/2f, -conversation_height);

            conversation_height += breakBar.GetComponent<RectTransform>().rect.height + message_buffer;
            conversation_height += 50;
            GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;
            LastMessage = null;
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
        public IEnumerator RevealOptions(DSDialogue dialogue, CharacterInfo speakingCharacter)
        {
            GameObject option_message = Instantiate(message_options, content_rect);
            option_message.transform.localPosition = new Vector2(content_rect.rect.width - right_buffer, -conversation_height);
            option_message.transform.localScale = Vector3.one;
            option_message.transform.localRotation = Quaternion.identity;
            MessageOptionsScript messageOptionScript = option_message.GetComponent<MessageOptionsScript>();
            messageOptionScript.options = Choices;
            messageOptionScript.CreateButtons();
            conversation_height += messageOptionScript.height + message_buffer;
            content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, conversation_height + end_buffer);
            GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;
            if (MessagingVariables.ForceSelect)
            {
                dialogue.setChoice(Choices[0]);
                UpdateTextHistory(speakingCharacter, "<b>" + Choices[0] + "\n");
            } else
            {
                yield return StartCoroutine(messageOptionScript.WaitForResponse());
                dialogue.setChoice(messageOptionScript.message);
                UpdateTextHistory(speakingCharacter, "<b>" + messageOptionScript.message + "\n");
            }
            GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;
            WaitingForChoice = false;
        }
        public IEnumerator WaitForChoiceSelection()
        {
            while (WaitingForChoice)
            {
                yield return null;
            }
        }

        public void NotificationPing()
        {
            notification_source.clip = new_message_notification;
            notification_source.Play();
        }
        public void UpdateTextHistory(CharacterInfo targetCharacter, string newText)
        {
            if(!MessageHistorys.Keys.Contains(targetCharacter)){
                MessageHistorys.Add(targetCharacter, "");
            }
            MessageHistorys[targetCharacter] += newText;
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

        public MessageBoxScript MakeLeftMessage(string message_text)
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

            message_info.SetSprite(CurrentCharacter.defaultCharacterSprite);
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
            RecreateMessages(CurrentCharacter);
        }

        private void RecreateMessages(CharacterInfo selectedCharacter)
        {
            if (!MessageHistorys.Keys.Contains(selectedCharacter)) return;

            string[] messageHistory = MessageHistorys[selectedCharacter].Split("\n");
            messageHistory = messageHistory.Skip(Math.Max(0, messageHistory.Length - 10)).ToArray();

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
                else if (message_source == "c")
                {
                    MakeDivisionBar();
                }
            }
        }
    }
}
