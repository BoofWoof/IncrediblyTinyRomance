using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using DS.Data.Save;
    using DS.Windows;
    using Enumerations;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using Utilities;

    public class DSSingleChoiceNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.SingleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceData);

            SpeakerInfo = new DSSpeakerData();
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

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");

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

            extensionContainer.Add(customDataContainer);

            DrawHorizontalLine();

            AddSkipBoolean();

            DrawHorizontalLine();

            AddCharacterFoldout();

            DrawHorizontalLine();

            MakeVariableFoldout();

            DrawHorizontalLine();

            AddReturnValue();

            DrawHorizontalLine();

            RefreshExpandedState();
        }
    }
}
