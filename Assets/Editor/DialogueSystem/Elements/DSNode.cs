using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace DS.Elements
{
    using DS.Data.Save;
    using DS.Windows;
    using Enumerations;
    using System;
    using UnityEditor.UIElements;
    using UnityEditor;
    using Utilities;

    public class DSNode : Node
    {
        public string ID {  get; set; }
        public string DialogueName { get; set; }
        public List<DSChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public bool SkipText { get; set; }
        public DSDialogueType DialogueType { get; set; }
        public DSGroup Group { get; set; }
        public DSSpeakerData SpeakerInfo { get; set; }
        public DSVariableData DialogueVariableInfo = null;
        public DSCheckVariableData DialogueCheckVariableInfo = null;
        public DSReturnVariableData DialogueReturnVariableInfo = null;

        private Foldout memorySettingFoldout = null;
        private Foldout returnValueSettingFoldout = null;

        protected DSGraphView graphView;
        private Color defaultBackgroundColor;

        public virtual void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            Choices = new List<DSChoiceSaveData>();
            Text = "Dialogue text.";
            SkipText = false;

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f/ 255f, 29f/ 255f, 30f/ 255f);

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");

            memorySettingFoldout = DSElementUtility.CreateFoldout("Settings");
            returnValueSettingFoldout = DSElementUtility.CreateFoldout("Settings");

            DialogueVariableInfo = new DSVariableData();
            DialogueReturnVariableInfo = new DSReturnVariableData();
        }
        #region Overrided Methods
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnet Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnet Output Ports", actionEvent => DisconnectOutputPorts());
            base.BuildContextualMenu(evt);
        }
        #endregion

        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if(!string.IsNullOrEmpty(DialogueName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    DialogueName = target.value;

                    graphView.AddUngroupedNode(this);
                    return;
                }

                DSGroup currentGroup = Group;

                graphView.RemoveGroupedNode(this, Group);

                DialogueName = callback.newValue;

                graphView.AddGroupedNode(this, currentGroup);
                return;
            });

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__filename-text-field",
                "ds-node__text-field_hidden"
            );

            titleContainer.Insert(0, dialogueNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);
        }
        #region Utility Methods
        public void DrawHorizontalLine()
        {
            VisualElement space = new VisualElement();
            space.style.height = 10; // Set the height of the space
            extensionContainer.Add(space);

            VisualElement horizontalLine = new VisualElement();
            horizontalLine.style.flexDirection = FlexDirection.Row;
            horizontalLine.style.height = 1;
            horizontalLine.style.backgroundColor = new StyleColor(Color.grey);

            // Add the horizontal line to the node
            extensionContainer.Add(horizontalLine);

            VisualElement space2 = new VisualElement();
            space2.style.height = 10; // Set the height of the space
            extensionContainer.Add(space2);
        }
        public void DisconnectallPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }
        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }
        private void DisconnectPorts(VisualElement container)
        {
            foreach(Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }
                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion

        public void AddSkipBoolean()
        {
            Toggle skipBoolean = new Toggle("Skip Dialogue:")
            {
                value = SkipText
            };
            skipBoolean.RegisterValueChangedCallback(evt =>
            {
                SkipText = evt.newValue;
            });
            extensionContainer.Add(skipBoolean);
        }

        #region ReturnFoldout
        public void AddReturnValue()
        {
            Foldout returnValueFoldout = DSElementUtility.CreateFoldout("Return Value", collapsed: true);

            //SAVE DATA
            DialogueReturnValue returnValueAsset = null;
            if (DialogueReturnVariableInfo != null)
            {
                string returnValueInfoPath = AssetDatabase.GUIDToAssetPath(DialogueReturnVariableInfo.ReturnValueInfoGUID);
                returnValueAsset = AssetDatabase.LoadAssetAtPath<DialogueReturnValue>(returnValueInfoPath);
            }
            ObjectField returnValueObjectField = new ObjectField()
            {
                value = returnValueAsset,
                objectType = typeof(DialogueReturnValue),
                allowSceneObjects = false
            };
            returnValueFoldout.Add(new Label("Return Value Type:"));
            returnValueFoldout.Add(returnValueObjectField);

            if (returnValueAsset != null)
            {
                updateReturnValueFoldout(returnValueAsset);
            }
            returnValueObjectField.RegisterValueChangedCallback(
                evt =>
                {
                    DialogueReturnValue returnValueAsset = (DialogueReturnValue)evt.newValue;
                    if (returnValueAsset != null)
                    {
                        DialogueReturnVariableInfo.TypeUuid = returnValueAsset.StateUuids[0];
                        DialogueReturnVariableInfo.ReturnValueInfoGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue));
                        updateReturnValueFoldout(returnValueAsset);
                    }
                }
            );

            returnValueFoldout.Add(returnValueSettingFoldout);
            extensionContainer.Add(returnValueFoldout);
        }

        private void updateReturnValueFoldout(DialogueReturnValue returnValueAsset)
        {
            returnValueSettingFoldout.Clear();

            if (returnValueAsset == null)
            {
                returnValueSettingFoldout.Add(new Label("Select A Return Type For More Settings"));
                return;
            }

            int popup_idx = 0;
            if (returnValueAsset.StateUuids.Contains(DialogueReturnVariableInfo.TypeUuid))
            {
                popup_idx = returnValueAsset.StateUuids.IndexOf(DialogueReturnVariableInfo.TypeUuid);
            }
            PopupField<string> popupField = new PopupField<string>(
                "Return Value Type",
                returnValueAsset.VariableStates,
                popup_idx
                );
            popupField.RegisterValueChangedCallback(evt =>
            {
                int optionIdx = ((PopupField<string>)evt.currentTarget).choices.IndexOf(evt.newValue);
                DialogueReturnVariableInfo.TypeUuid = returnValueAsset.StateUuids[optionIdx];
            });
            returnValueSettingFoldout.Add(popupField);

            FloatField returnValue = new FloatField("Return Value:") { 
                value = DialogueReturnVariableInfo.ReturnValue
            };
            returnValue.RegisterValueChangedCallback(evt =>
            {
                DialogueReturnVariableInfo.ReturnValue = evt.newValue;
            });
            returnValueSettingFoldout.Add(returnValue);
        }
        #endregion

        #region VariableFoldout
        public void MakeVariableFoldout()
        {
            Foldout memoryFoldout = DSElementUtility.CreateFoldout("Memory Set", collapsed: true);

            //SAVE DATA
            DialogueOptionsVariable dialogueVariableAsset = null;
            if (DialogueVariableInfo != null)
            {
                string dialogueVariableInfoPath = AssetDatabase.GUIDToAssetPath(DialogueVariableInfo.VariableInfoGUID);
                dialogueVariableAsset = AssetDatabase.LoadAssetAtPath<DialogueOptionsVariable>(dialogueVariableInfoPath);
            }
            ObjectField variableObjectField = new ObjectField()
            {
                value = dialogueVariableAsset,
                objectType = typeof(DialogueOptionsVariable),
                allowSceneObjects = false
            };
            memoryFoldout.Add(new Label("Variable Object:"));
            memoryFoldout.Add(variableObjectField);

            if (dialogueVariableAsset != null)
            {
                updateVariableFoldout(dialogueVariableAsset);
            }
            variableObjectField.RegisterValueChangedCallback(
                evt =>
                {
                    DialogueOptionsVariable dialogueVariableAsset = (DialogueOptionsVariable)evt.newValue;
                    DialogueVariableInfo.VariableInfoGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue));
                    updateVariableFoldout(dialogueVariableAsset);
                }
            );

            memoryFoldout.Add(memorySettingFoldout);
            extensionContainer.Add(memoryFoldout);
        }

        private void updateVariableFoldout(DialogueOptionsVariable dialogueoptionsVariable)
        {
            memorySettingFoldout.Clear();

            if (dialogueoptionsVariable == null)
            {
                memorySettingFoldout.Add(new Label("Select A Variable Object For More Settings"));
                return;
            }

            switch (dialogueoptionsVariable.VariableType)
            {
                case (VariableTypeEnum.Value):
                    {
                        DrawValueFields(memorySettingFoldout);
                        break;
                    }
                case (VariableTypeEnum.Option):
                    {
                        DrawOptionFields(memorySettingFoldout, dialogueoptionsVariable);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void DrawValueFields(Foldout variableFoldout)
        {
            EnumField operandField = new EnumField("Operand Type:", DialogueVariableInfo.OperandType);
            operandField.RegisterValueChangedCallback(evt =>
            {
                DialogueVariableInfo.OperandType = (OperandTypeEnum)evt.newValue;
            });
            variableFoldout.Add(operandField);

            FloatField operandValueField = new FloatField("Operand Value:") { value = DialogueVariableInfo.OperandValue };
            operandValueField.RegisterValueChangedCallback(evt =>
            {
                DialogueVariableInfo.OperandValue = evt.newValue;
            });
            variableFoldout.Add(operandValueField);
        }

        private void DrawOptionFields(Foldout variableFoldout, DialogueOptionsVariable dialogueoptionsVariable)
        {
            int popup_idx = dialogueoptionsVariable.StateUuids.IndexOf(dialogueoptionsVariable.StartingUuid);
            if (dialogueoptionsVariable.StateUuids.Contains(DialogueVariableInfo.OptionUid))
            {
                popup_idx = dialogueoptionsVariable.StateUuids.IndexOf(DialogueVariableInfo.OptionUid);
            }
            DialogueVariableInfo.OptionUid = dialogueoptionsVariable.StateUuids[popup_idx];
            PopupField<string> popupField = new PopupField<string>(
                "Memory Setting",
                dialogueoptionsVariable.VariableStates,
                popup_idx
                );
            popupField.RegisterValueChangedCallback(evt =>
            {
                int optionIdx = ((PopupField<string>)evt.currentTarget).choices.IndexOf(evt.newValue);
                DialogueVariableInfo.OptionUid = dialogueoptionsVariable.StateUuids[optionIdx];
            });
            variableFoldout.Add(popupField);
        }
        #endregion

        #region CharacterFoldout
        public void AddCharacterFoldout()
        {
            Foldout speakerFoldout = DSElementUtility.CreateFoldout("Speaker Info", collapsed: true);

            //Add Character Data
            CharacterInfo characterInfoAsset = null;
            if (SpeakerInfo != null)
            {
                string characterInfoAssetPath = AssetDatabase.GUIDToAssetPath(SpeakerInfo.CharacterInfoGUID);
                characterInfoAsset = AssetDatabase.LoadAssetAtPath<CharacterInfo>(characterInfoAssetPath);
            }
            ObjectField characterField = new ObjectField()
            {
                value = characterInfoAsset,
                objectType = typeof(CharacterInfo),
                allowSceneObjects = false
            };

            PopupField<string> popupSpriteField = null;
            PopupField<string> popupNoiseField = null;

            characterField.RegisterValueChangedCallback(
            evt =>
            {
                CharacterInfo characterInfo = (CharacterInfo)evt.newValue;
                if (characterInfo == null)
                {
                    if (popupSpriteField != null) speakerFoldout.Remove(popupSpriteField);
                    if (popupNoiseField != null) speakerFoldout.Remove(popupNoiseField);
                    SpeakerInfo.CharacterInfoGUID = null;
                    SpeakerInfo.SpriteUid = null;
                    SpeakerInfo.NoiseUid = null;
                    return;
                }

                SpeakerInfo.CharacterInfoGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue));

                if (popupSpriteField != null) speakerFoldout.Remove(popupSpriteField);
                popupSpriteField = CreatePopupField("Sprite", characterInfo.emotionSpriteNames, characterInfo.spriteUuid);
                popupSpriteField.RegisterValueChangedCallback(evt2 =>
                {
                    int uuidIdx = popupSpriteField.choices.IndexOf(evt2.newValue) - 1;
                    SpeakerInfo.SpriteUid = uuidIdx != -1 ? characterInfo.spriteUuid[uuidIdx] : null;
                });
                speakerFoldout.Add(popupSpriteField);

                if (popupNoiseField != null) speakerFoldout.Remove(popupNoiseField);
                popupNoiseField = CreatePopupField("Noise", characterInfo.emotionNoiseNames, characterInfo.noiseUuid);
                popupNoiseField.RegisterValueChangedCallback(evt2 =>
                {
                    int uuidIdx = popupNoiseField.choices.IndexOf(evt2.newValue) - 1;
                    SpeakerInfo.NoiseUid = uuidIdx != -1 ? characterInfo.noiseUuid[uuidIdx] : null;
                });
                speakerFoldout.Add(popupNoiseField);
            }
            );

            speakerFoldout.Add(characterField);

            if (characterInfoAsset != null)
            {
                popupSpriteField = CreatePopupField("Sprite", characterInfoAsset.emotionSpriteNames, characterInfoAsset.spriteUuid, SpeakerInfo.SpriteUid);
                popupSpriteField.RegisterValueChangedCallback(evt2 =>
                {
                    int uuidIdx = popupSpriteField.choices.IndexOf(evt2.newValue) - 1;
                    SpeakerInfo.SpriteUid = uuidIdx != -1 ? characterInfoAsset.spriteUuid[uuidIdx] : null;
                });
                speakerFoldout.Add(popupSpriteField);
                popupNoiseField = CreatePopupField("Noise", characterInfoAsset.emotionNoiseNames, characterInfoAsset.noiseUuid, SpeakerInfo.NoiseUid);
                popupNoiseField.RegisterValueChangedCallback(evt2 =>
                {
                    int uuidIdx = popupNoiseField.choices.IndexOf(evt2.newValue) - 1;
                    SpeakerInfo.NoiseUid = uuidIdx != -1 ? characterInfoAsset.noiseUuid[uuidIdx] : null;
                });
                speakerFoldout.Add(popupNoiseField);
            }
            extensionContainer.Add(speakerFoldout);
        }

        private PopupField<string> CreatePopupField(string label, List<string> options, List<string> uuids, string startingUuid = null)
        {
            int startingIdx = 0;
            if (startingUuid != null && uuids.Contains(startingUuid))
            {
                startingIdx = uuids.IndexOf(startingUuid) + 1;
            }
            PopupField<string> popupField;
            List<string> optionList = new List<string>(options);
            optionList.Insert(0, "Default");
            popupField = new PopupField<string>(label, optionList, startingIdx);
            return popupField;
        }
        #endregion
    }
}
