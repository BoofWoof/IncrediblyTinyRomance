using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DS
{
    using DS.Data;
    using ScriptableObjects;
    using System;

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
        public DSReturnValueInfo getReturnValue()
        {
            return dialogue.ReturnValueInfo;
        }
        public void setChoice(string selected_choice)
        {
            if (dialogue.Choices.Count == 1)
            {
                dialogue = dialogue.Choices[0].NextDialogue;
                dialogueChecks();
                return;
            }
            foreach (DSDialogueChoiceData choice in dialogue.Choices)
            {
                if (selected_choice == choice.Text)
                {
                    dialogue = choice.NextDialogue;
                    dialogueChecks();
                    return;
                }
            }
            Debug.LogWarning("No matching choice found.");
        }

        private void dialogueChecks()
        {
            if (dialogue == null) return;
            memoryUpdateCheck();
            autoProgressNodeCheck();
        }

        private void memoryUpdateCheck()
        {
            if(dialogue.VariableInfo.VariableInfoSO == null)
            {
                return;
            }
            DialogueOptionsVariable targetVariable = dialogue.VariableInfo.VariableInfoSO;
            DSVariableInfo variableSettings = dialogue.VariableInfo;
            string variableID = targetVariable.uniqueID;

            DSMemory.InitializeVariable(targetVariable);

            switch (targetVariable.VariableType)
            {
                case VariableTypeEnum.Value:
                    {
                        switch (variableSettings.OperandType)
                        {
                            case OperandTypeEnum.Set:
                                {
                                    DSMemory.ValueMemory[variableID] = variableSettings.OperandValue;
                                    break;
                                }
                            case OperandTypeEnum.Add:
                                {
                                    DSMemory.ValueMemory[variableID] += variableSettings.OperandValue;
                                    break;
                                }
                        }
                        break;
                    }
                case VariableTypeEnum.Option:
                    {
                        DSMemory.OptionMemory[variableID] = variableSettings.OptionUid;
                        break;
                    }
            }
        }

        public void autoProgressNodeCheck()
        {
            if(dialogue.DialogueType == Enumerations.DSDialogueType.CheckVariable)
            {
                DialogueOptionsVariable targetVariable = dialogue.CheckInfo.VariableInfoSO;
                DSMemory.InitializeVariable(targetVariable);
                switch (targetVariable.VariableType)
                {
                    case VariableTypeEnum.Value:
                        {
                            if (DSMemory.ValueMemory[targetVariable.uniqueID] < dialogue.CheckInfo.ThresholdValue)
                            {
                                dialogue = dialogue.Choices[0].NextDialogue;
                            } else
                            {
                                dialogue = dialogue.Choices[1].NextDialogue;
                            }
                            dialogueChecks();
                            break;
                        }
                    case VariableTypeEnum.Option:
                        {
                            foreach (DSDialogueChoiceData choice in dialogue.Choices)
                            {
                                if (choice.NextDialogueUuid == DSMemory.OptionMemory[targetVariable.uniqueID])
                                {
                                    Debug.Log("You did it!");
                                    dialogue = choice.NextDialogue;
                                    dialogueChecks();
                                    return;
                                }
                            }
                            break;
                        }
                }
            }
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