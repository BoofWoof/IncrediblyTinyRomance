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
                        if (popupSpriteField != null) extensionContainer.Remove(popupSpriteField);
                        if (popupNoiseField != null) extensionContainer.Remove(popupNoiseField);
                        SpeakerInfo.CharacterInfoGUID = null;
                        SpeakerInfo.SpriteUid = null;
                        SpeakerInfo.NoiseUid = null;
                        return;
                    }

                    SpeakerInfo.CharacterInfoGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(evt.newValue));
                    Debug.Log(SpeakerInfo.CharacterInfoGUID);

                    if (popupSpriteField != null) extensionContainer.Remove(popupSpriteField);
                    popupSpriteField = CreatePopupField("Sprite", characterInfo.emotionSpriteNames, characterInfo.spriteUuid);
                    popupSpriteField.RegisterValueChangedCallback(evt2 =>
                    {
                        int uuidIdx = popupSpriteField.choices.IndexOf(evt2.newValue) - 1;
                        SpeakerInfo.SpriteUid = uuidIdx != -1 ? characterInfo.spriteUuid[uuidIdx] : null;
                    });
                    extensionContainer.Add(popupSpriteField);

                    if (popupNoiseField != null) extensionContainer.Remove(popupNoiseField);
                    popupNoiseField = CreatePopupField("Noise", characterInfo.emotionNoiseNames, characterInfo.noiseUuid);
                    popupNoiseField.RegisterValueChangedCallback(evt2 =>
                    {
                        int uuidIdx = popupNoiseField.choices.IndexOf(evt2.newValue) - 1;
                        SpeakerInfo.NoiseUid = uuidIdx != -1 ? characterInfo.noiseUuid[uuidIdx] : null;
                    });
                    extensionContainer.Add(popupNoiseField);
                }
                );

            extensionContainer.Add(characterField);

            if (characterInfoAsset != null)
            {
                popupSpriteField = CreatePopupField("Sprite", characterInfoAsset.emotionSpriteNames, characterInfoAsset.spriteUuid, SpeakerInfo.SpriteUid);
                popupSpriteField.RegisterValueChangedCallback(evt2 =>
                {
                    int uuidIdx = popupSpriteField.choices.IndexOf(evt2.newValue) - 1;
                    SpeakerInfo.SpriteUid = uuidIdx != -1 ? characterInfoAsset.spriteUuid[uuidIdx] : null;
                });
                extensionContainer.Add(popupSpriteField);
                popupNoiseField = CreatePopupField("Noise", characterInfoAsset.emotionNoiseNames, characterInfoAsset.noiseUuid, SpeakerInfo.NoiseUid);
                popupNoiseField.RegisterValueChangedCallback(evt2 =>
                {
                    int uuidIdx = popupNoiseField.choices.IndexOf(evt2.newValue) - 1;
                    SpeakerInfo.NoiseUid = uuidIdx != -1 ? characterInfoAsset.noiseUuid[uuidIdx] : null;
                });
                extensionContainer.Add(popupNoiseField);
            }

            RefreshExpandedState();
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
    }
}
