using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using DS.Data.Save;
    using DS.Windows;
    using Enumerations;
    using UnityEditor;
    using UnityEditor.UIElements;
    using Utilities;
    public class DSSaveNode : DSNode
    {
        private Foldout saveFoldout = null;
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.SaveVariable;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceData);

            Text = "Make notes here. This text does not show up in game.";

            DialogueVariableInfo = new DSVariableData();

            saveFoldout = DSElementUtility.CreateFoldout("Save Data");
        }
        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            /* Extension Data Container */

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Save Note");

            TextField textTextField = DSElementUtility.CreateTextArea(Text, null, callback =>
            {
                Text = callback.newValue;
            });

            textTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__quote-text-field"
            );

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

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
            customDataContainer.Add(new Label("Variable Object:"));
            customDataContainer.Add(variableObjectField);

            if (dialogueVariableAsset != null)
            {
                updateVariableFoldout(dialogueVariableAsset);
            }
            variableObjectField.RegisterValueChangedCallback(
                evt =>
                {
                    DialogueOptionsVariable dialogueVariableAsset = (DialogueOptionsVariable)evt.newValue;
                    DialogueVariableInfo.VariableInfoGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue));
                    if (dialogueVariableAsset != null)
                    {
                        updateVariableFoldout(dialogueVariableAsset);
                    }
                }
            );

            customDataContainer.Add(saveFoldout);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }

        private void updateVariableFoldout(DialogueOptionsVariable dialogueoptionsVariable)
        {
            saveFoldout.Clear();

            switch (dialogueoptionsVariable.VariableType)
            {
                case (VariableTypeEnum.Value):
                    {
                        DrawValueFields(saveFoldout);
                        break;
                    }
                case (VariableTypeEnum.Option):
                    {
                        DrawOptionFields(saveFoldout, dialogueoptionsVariable);
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
            if (dialogueoptionsVariable.StateUuids.Contains(DialogueVariableInfo.OptionUid)){
                popup_idx = dialogueoptionsVariable.StateUuids.IndexOf(DialogueVariableInfo.OptionUid);
            }
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
    }
}
