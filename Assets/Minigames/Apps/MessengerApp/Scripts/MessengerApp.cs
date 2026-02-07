using PixelCrushers.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace DS
{
    public struct ChoicePair
    {
        public int SourceID;
        public Response[] Choices;
    }

    public class MessengerApp : AppScript
    {
        [HideInInspector] private PixelCrushers.DialogueSystem.CharacterInfo CurrentCharacter;
        [HideInInspector] public List<int> UncheckMessages = new List<int>();

        public static MessengerApp instance;

        public Sprite NotificationSprite;

        [Header("Objects")]
        public ScrollRect targetScrollRect;
        public RectTransform content_rect;

        [Header("Prefabs")]
        public GameObject LeftMessagePrefab;
        public GameObject RightMessagePrefab;
        public GameObject OptionMessagePrefab;
        public GameObject DivisionBarPrefab;

        [Header("Data")]
        private Dictionary<int, string> MessageHistorys = new Dictionary<int, string>();

        [HideInInspector] public ChoicePair Choices;

        [Header("NotificationSounds")]
        public AudioSource notification_source;

        public int ShowMessageHistory = 20;

        private GameObject LastLeftMessage;
        private bool DoubleMessage = false;
        private bool NewObjectAdded = false;

        [HideInInspector] public int AppSong = -1;
        [HideInInspector] public bool SongLock = false;

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            Lua.RegisterFunction("SetTextSong", this, SymbolExtensions.GetMethodInfo(() => SetTextSongLUA(0f)));
            Lua.RegisterFunction("LockInSong", this, SymbolExtensions.GetMethodInfo(() => LockInSong()));
            Lua.RegisterFunction("ReleaseSong", this, SymbolExtensions.GetMethodInfo(() => ReleaseSong()));

            Choices = new ChoicePair
            {
                SourceID = -1,
                Choices = null
            };
        }

        public void LateUpdate()
        {
            if (NewObjectAdded)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(content_rect.GetComponent<RectTransform>());
                targetScrollRect.verticalNormalizedPosition = 0f;
            }
            NewObjectAdded = false;
        }

        public void SetCharacter(PixelCrushers.DialogueSystem.CharacterInfo newCharacter)
        {
            if (CurrentCharacter != null && newCharacter.id == CurrentCharacter.id) return;
            CurrentCharacter = newCharacter;
            RecreateFromText();
        }
        public void RecreateFromText()
        {
            ResetMessanger();
            RecreateMessages(CurrentCharacter);
        }
        public void ResetMessanger()
        {
            foreach (Transform child in content_rect)
            {
                Destroy(child.gameObject);
            }
            DoubleMessage = false;
        }
        public void UpdateTextHistory(int targetCharacterID, string newText)
        {
            if (MessageHistorys == null) return;
            if(!MessageHistorys.Keys.Contains(targetCharacterID)){
                MessageHistorys.Add(targetCharacterID, "");
            }
            MessageHistorys[targetCharacterID] += newText;
        }

        public void AddUncheckedMessage(int speakerId)
        {
            if (!UncheckMessages.Contains(speakerId))
            {
                UncheckMessages.Add(speakerId);
                NotificationMenuScript.SetNotification("Messenger", NotificationSprite);
            }
        }

        public void ClearNotifications()
        {
            if (UncheckMessages == null || CurrentCharacter == null) return;
            UncheckMessages.Remove(CurrentCharacter.id);
            if (UncheckMessages.Count == 0)
            {
                NotificationMenuScript.ReleaseNotification("Messenger");
            }
        }

        public void CheckNotifications()
        {
            if (UncheckMessages == null) return;
            if (UncheckMessages.Count > 0  || Choices.SourceID != -1)
            {
                NotificationMenuScript.SetNotification("Messenger", NotificationSprite);
            } else
            {
                NotificationMenuScript.ReleaseNotification("Messenger");
            }
        }

        public void AddLeftMessage(int speakerId, string message_text)
        {
            UpdateTextHistory(speakerId, "<a>" + message_text + "\n");
            if (CurrentCharacter != null && speakerId == CurrentCharacter.id) {
                MakeLeftMessage(message_text);
            }
            else {
                AddUncheckedMessage(speakerId);
            };
            if(!Active) AddUncheckedMessage(speakerId);
            notification_source.Play();
        }
        private void MakeLeftMessage(string message_text)
        {
            GameObject newObject = AddObjectToContainer(LeftMessagePrefab);
            newObject.GetComponent<MessageBoxScript>().SetText(message_text);
            newObject.GetComponent<MessageBoxScript>().SetSprite(CurrentCharacter.portrait);

            if(DoubleMessage) LastLeftMessage.GetComponent<MessageBoxScript>().ReplaceBox();
            LastLeftMessage = newObject;

            DoubleMessage = true;

            NewObjectAdded = true;
        }

        private void AddRightMessage(int speakerId, string message_text)
        {
            UpdateTextHistory(speakerId, "<b>" + message_text + "\n");
            if (CurrentCharacter != null && speakerId == CurrentCharacter.id) MakeRightMessage(message_text);
        }

        private void MakeRightMessage(string message_text)
        {
            GameObject newObject = AddObjectToContainer(RightMessagePrefab);
            newObject.GetComponent<MessageBoxScript>().SetText(message_text);

            DoubleMessage = false;

            NewObjectAdded = true;
        }
        public void SendOptions(int speakerId, Response[] responseOptions)
        {
            if (MessagingVariables.ForceSelect)
            {
                (DialogueManager.dialogueUI as AbstractDialogueUI).OnClick(responseOptions[0]);
                AddRightMessage(speakerId, responseOptions[0].formattedText.text);
                return;
            }
            Choices = new ChoicePair
            {
                Choices = responseOptions,
                SourceID = speakerId
            };
            if (CurrentCharacter != null && speakerId == CurrentCharacter.id)
            {
                ShowOptions();
            }
            else
            {
                if (!UncheckMessages.Contains(speakerId)) UncheckMessages.Add(speakerId);
            };
        }

        public void ShowOptions()
        {
            GameObject newObject = AddObjectToContainer(OptionMessagePrefab);
            newObject.GetComponent<MessageOptionsScript>().CreateButtons(Choices.Choices);

            NewObjectAdded = true;
        }

        public void SelectOption(int optionIdx)
        {
            int targetID = Choices.SourceID;
            Response[] targetOptions = Choices.Choices;
            (DialogueManager.dialogueUI as AbstractDialogueUI).OnClick(targetOptions[optionIdx]);
            AddRightMessage(targetID, targetOptions[optionIdx].formattedText.text);

            Choices = new ChoicePair
            {
                SourceID = -1,
                Choices = null
            };
        }

        public void AddDivisionBar(int speakerId)
        {
            UpdateTextHistory(speakerId, "<c>" + "\n");
            if (CurrentCharacter != null && speakerId == CurrentCharacter.id) MakeDivisionBar();
        }

        public void MakeDivisionBar()
        {
            AddObjectToContainer(DivisionBarPrefab);

            DoubleMessage = false;

            targetScrollRect.verticalNormalizedPosition = 1f;
        }
        public GameObject AddObjectToContainer(GameObject targetPrefab)
        {
            GameObject newObject = Instantiate(targetPrefab, content_rect);
            newObject.transform.localScale = Vector3.one;
            newObject.transform.localRotation = Quaternion.identity;
            newObject.transform.localPosition = Vector3.zero;

            return newObject;
        }

        private void RecreateMessages(PixelCrushers.DialogueSystem.CharacterInfo selectedCharacter)
        {
            if (!MessageHistorys.Keys.Contains(selectedCharacter.id)) return;

            //Old version: MessageHistorys[selectedCharacter.id].Split("\n");
            string[] messageHistory = Regex.Split(MessageHistorys[selectedCharacter.id], @"\n(?=<)");
            messageHistory = messageHistory.Skip(Math.Max(0, messageHistory.Length - ShowMessageHistory)).ToArray();

            foreach (string message in messageHistory)
            {
                string pattern = @"(?<=\<).*?(?=\>)";
                string message_source = Regex.Match(message, pattern).Value;
                if (message_source == "a")
                {
                    string trimmed_message = message.Substring(3);
                    MakeLeftMessage(trimmed_message);
                } else if (message_source == "b")
                {
                    string trimmed_message = message.Substring(3);
                    MakeRightMessage(trimmed_message);
                }
                else if (message_source == "c")
                {
                    MakeDivisionBar();
                }
            }

            if (Choices.SourceID == selectedCharacter.id) ShowOptions();
        }

        #region Song
        public void OnEnable()
        {
            OnShowApp += MusinOnAppShow;
            OnHideApp += MusinOnAppHide;
        }

        public void OnDisable()
        {
            OnShowApp -= MusinOnAppShow;
            OnHideApp -= MusinOnAppHide;
        }
        public void LockInSong()
        {
            SongLock = true;
            if (Active)
            {
                MusicSelectorScript.LockSong();
            }
        }
        public void ReleaseSong()
        {
            SongLock = false;
            MusicSelectorScript.ReleaseSong();
        }
        public void SetTextSongLUA(float newAppSong)
        {
            SetTextSong((int)newAppSong);
        }
        public void SetTextSong(int newAppSong)
        {
            if (newAppSong == AppSong) return;
            AppSong = newAppSong;

            if (Active)
            {
                if (AppSong < 0)
                {
                    MusicSelectorScript.RevertPhoneSong();
                    return;
                }
                MusicSelectorScript.SetPhoneSong(AppSong);
            }
        }
        public void MusinOnAppShow()
        {
            if (AppSong < 0) return;
            MusicSelectorScript.SetPhoneSong(AppSong);
            if (SongLock) MusicSelectorScript.LockSong();
        }
        public void MusinOnAppHide()
        {
            MusicSelectorScript.RevertPhoneSong();
        }

        #endregion
    }
}
