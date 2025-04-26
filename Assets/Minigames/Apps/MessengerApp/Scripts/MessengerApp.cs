using JetBrains.Annotations;
using PixelCrushers.DialogueSystem;
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
        public GameObject audio_button_prefab;

        [Header("Data")]
        private Dictionary<int, string> MessageHistorys = new Dictionary<int, string>();
        public MessageBoxScript LastMessage = null;
        public bool LastLeft = false;

        public PixelCrushers.DialogueSystem.CharacterInfo CurrentCharacter;

        public Response[] Choices;
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

        [Header("Voice")]
        public AudioSource VoiceSource;

        public void MakeAudioMessage(string audioFileName)
        {
            audioFileName = audioFileName.CleanResourcePath();
            VoiceLineSO voiceLine = Resources.Load<VoiceLineSO>(audioFileName);

            GameObject audioButton = Instantiate(audio_button_prefab, Vector2.zero, Quaternion.identity);

            MessageAudioScript messageAudioScript = audioButton.GetComponent<MessageAudioScript>();
            messageAudioScript.MessageAudioSource = VoiceSource;
            messageAudioScript.VoiceLine = voiceLine;

            audioButton.transform.parent = content_rect;
            audioButton.transform.localScale = Vector3.one;
            audioButton.transform.localRotation = Quaternion.identity;

            float buttonHeight = audioButton.GetComponent<RectTransform>().rect.height;
            float buttonWidth = audioButton.GetComponent<RectTransform>().rect.width;
            audioButton.transform.localPosition = new Vector2(left_buffer + buttonWidth/2f, -conversation_height - 4 * message_buffer);
            conversation_height += buttonHeight + message_buffer;

            GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;
        }

        // Start is called before the first frame update
        void Awake()
        {
            conversation_height = start_buffer;
        }

        public void SetTextChoices(Response[] responses)
        {
            WaitingForChoice = true;
            Choices = responses;
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
        public IEnumerator RevealOptions(PixelCrushers.DialogueSystem.CharacterInfo speakingCharacter)
        {
            List<string> choiceText = new List<string>();
            foreach (Response response in Choices)
            {
                choiceText.Add(response.formattedText.text);
            }

            GameObject option_message = Instantiate(message_options, content_rect);
            Debug.Log(option_message);
            option_message.transform.localPosition = new Vector2(content_rect.rect.width - right_buffer, -conversation_height);
            option_message.transform.localScale = Vector3.one;
            option_message.transform.localRotation = Quaternion.identity;
            MessageOptionsScript messageOptionScript = option_message.GetComponent<MessageOptionsScript>();
            messageOptionScript.options = choiceText;
            messageOptionScript.CreateButtons();
            conversation_height += messageOptionScript.height + message_buffer;
            content_rect.sizeDelta = new Vector2(content_rect.sizeDelta.x, conversation_height + end_buffer);
            GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;

            int choiceIdx = 0;
            if (MessagingVariables.ForceSelect)
            {
                UpdateTextHistory(speakingCharacter, "<b>" + choiceText[0] + "\n");
            } else
            {
                yield return StartCoroutine(messageOptionScript.WaitForResponse());
                choiceIdx = messageOptionScript.OptionIdx;
                UpdateTextHistory(speakingCharacter, "<b>" + messageOptionScript.message + "\n");
            }
            GetComponentInChildren<ScrollRect>(content_rect).verticalNormalizedPosition = 0f;
            WaitingForChoice = false;

            RecreateFromText();
            (DialogueManager.dialogueUI as AbstractDialogueUI).OnClick(Choices[choiceIdx]);
        }

        public void NotificationPing()
        {
            notification_source.clip = new_message_notification;
            notification_source.Play();
        }
        public void UpdateTextHistory(PixelCrushers.DialogueSystem.CharacterInfo targetCharacter, string newText)
        {
            if(!MessageHistorys.Keys.Contains(targetCharacter.id)){
                MessageHistorys.Add(targetCharacter.id, "");
            }
            MessageHistorys[targetCharacter.id] += newText;
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

            message_info.SetSprite(CurrentCharacter.portrait);
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

        private void RecreateMessages(PixelCrushers.DialogueSystem.CharacterInfo selectedCharacter)
        {
            if (!MessageHistorys.Keys.Contains(selectedCharacter.id)) return;

            string[] messageHistory = MessageHistorys[selectedCharacter.id].Split("\n");
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
                else if (message_source == "v")
                {
                    string trimmed_message = message.Substring(3);
                    MakeAudioMessage(trimmed_message);
                }
            }
        }
    }
}
