using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS
{
    using DS.Data;
    using ScriptableObjects;
    public class DSDialogue : MonoBehaviour
    {
        /* Dialogue Scriptable Objects */
        [SerializeField] private DSDialogueContainerSO dialogueContainer;
        [SerializeField] private DSDialogueGroupSO dialogueGroup;
        [SerializeField] private DSDialogueSO dialogue;

        /* Filters */
        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;

        /* Indexes */
        [SerializeField] private int selectedDialogueGroupIndex;
        [SerializeField] private int selectedDialogueIndex;

        public string getText()
        {
            return dialogue.Text;
        }
        public List<string> getChoices()
        {
            List<string> choices = new List<string>();
            foreach (DSDialogueChoiceData choice in dialogue.Choices)
            {
                choices.Add(choice.Text);
            }
            return choices;
        }
        public void setChoice(string selected_choice)
        {
            if (dialogue.Choices.Count == 1)
            {
                dialogue = dialogue.Choices[0].NextDialogue;
                return;
            }
            foreach (DSDialogueChoiceData choice in dialogue.Choices)
            {
                if (selected_choice == choice.Text)
                {
                    dialogue = choice.NextDialogue;
                    return;
                }
            }
            Debug.LogWarning("No matching choice found.");
        }

        public Sprite getSprite()
        {
            CharacterInfo characterInfo = dialogue.SpeakerInfo.CharacterInfoSO;
            if (characterInfo == null || !characterInfo.spriteUuid.Contains(dialogue.SpeakerInfo.SpriteUid)) return null;
            return characterInfo.emotionSprites[characterInfo.spriteUuid.IndexOf(dialogue.SpeakerInfo.SpriteUid)];
        }
        public AudioClip getNoise()
        {
            CharacterInfo characterInfo = dialogue.SpeakerInfo.CharacterInfoSO;
            if (characterInfo == null || !characterInfo.noiseUuid.Contains(dialogue.SpeakerInfo.NoiseUid)) return null;
            return characterInfo.emotionNoises[characterInfo.noiseUuid.IndexOf(dialogue.SpeakerInfo.NoiseUid)];
        }

        public bool isDone()
        {
            return dialogue == null;
        }

        public bool isSingleOption()
        {
            return dialogue.DialogueType == Enumerations.DSDialogueType.SingleChoice;
        }
        public bool isMultipleOptions()
        {
            return dialogue.DialogueType == Enumerations.DSDialogueType.MultipleChoice;
        }
    }
}