using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using DS.Data.Save;
    using DS.Windows;
    using Enumerations;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using Utilities;
    public class DSCheckNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.CheckVariable;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceData);

            Text = "Make notes here. This text does not show up in game.";

            DialogueCheckVariableInfo = new DSCheckVariableData();

            SkipText = true;
        }
        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */

            /* Extension Data Container */

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Check Note");

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
                string dialogueVariableInfoPath = AssetDatabase.GUIDToAssetPath(DialogueCheckVariableInfo.VariableInfoGUID);
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
                updatePorts(dialogueVariableAsset, false);
            }
            variableObjectField.RegisterValueChangedCallback(
                evt =>
                {
                    DialogueOptionsVariable dialogueVariableAsset = (DialogueOptionsVariable)evt.newValue;
                    DialogueCheckVariableInfo.VariableInfoGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue));
                    if (dialogueVariableAsset != null)
                    {
                        updatePorts(dialogueVariableAsset);
                    }
                }
            );

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }

        private void updatePorts(DialogueOptionsVariable dialogueVariableAsset, bool clearContainer = true)
        {
            if (clearContainer)
            {
                DeletePorts();
            }

            switch (dialogueVariableAsset.VariableType)
            {
                case (VariableTypeEnum.Value):
                    {
                        addValuePorts(dialogueVariableAsset);
                        break;
                    }
                case (VariableTypeEnum.Option):
                    {
                        addOptionPorts(dialogueVariableAsset);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void addOptionPorts(DialogueOptionsVariable dialogueVariableAsset)
        {
            List<DSChoiceSaveData> newChoices = new List<DSChoiceSaveData>();
            for (int i = 0; i < dialogueVariableAsset.VariableStates.Count; i++)
            {
                string choiceText = dialogueVariableAsset.VariableStates[i];
                string choiceUUID = dialogueVariableAsset.StateUuids[i];


                DSChoiceSaveData storedChoice = null;
                foreach (DSChoiceSaveData choice in Choices)
                {
                    if(choice.OptionUUID == choiceUUID)
                    {
                        storedChoice = choice;
                        newChoices.Add(choice);
                        break;
                    }
                }

                if (storedChoice == null)
                {
                    newChoices.Add(new DSChoiceSaveData()
                    {
                        Text = choiceText,
                        OptionUUID = choiceUUID
                    });
                }
            }
            Choices = newChoices;

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }
            RefreshExpandedState();
        }

        private void addValuePorts(DialogueOptionsVariable dialogueVariableAsset)
        {
            //Make Choices
            if (Choices.Count != 2 || Choices[0].Text != "<" || Choices[1].Text != ">=")
            {
                List<DSChoiceSaveData> newChoices = new List<DSChoiceSaveData>();
                DSChoiceSaveData choice1 = new DSChoiceSaveData()
                {
                    Text = "<"
                };
                newChoices.Add(choice1);

                DSChoiceSaveData choice2 = new DSChoiceSaveData()
                {
                    Text = ">="
                };
                newChoices.Add(choice2);

                Choices = newChoices;
            }

            //Add choice ports.
            Port choicePort1 = this.CreatePort(Choices[0].Text);

            choicePort1.userData = Choices[0];

            Port choicePort2 = this.CreatePort(Choices[1].Text);

            choicePort2.userData = Choices[1];

            //Threatshold
            FloatField threshold = new FloatField("Threshold:")
            {
                value = DialogueCheckVariableInfo.ThresholdValue
            };
            threshold.RegisterValueChangedCallback(
                evt =>
                {
                    DialogueCheckVariableInfo.ThresholdValue = evt.newValue;
                }
            );

            outputContainer.Add(choicePort1);

            outputContainer.Add(threshold);

            outputContainer.Add(choicePort2);

            RefreshExpandedState();
        }

        private void DeletePorts()
        {
            Choices.Clear();

            List<Port> portsToDelete = new List<Port>();

            foreach (var choicePort in outputContainer.Children())
            {
                if (choicePort is Port)
                {
                    portsToDelete.Add((Port)choicePort);
                }
            }
            foreach (Port choicePort in portsToDelete)
            {
                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }
                graphView.RemoveElement(choicePort);
            }
            outputContainer.Clear();
        }
    }
}