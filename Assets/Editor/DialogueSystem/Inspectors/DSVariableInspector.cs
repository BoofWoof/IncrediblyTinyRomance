using DS.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace DS.DialogueVariables
{
    [CustomEditor(typeof(DialogueOptionsVariable))]
    public class DSVariableInspector : Editor
    {
        DialogueOptionsVariable dialogueOptionsVariable;

        private SerializedProperty VariableName;
        private SerializedProperty VariableType;
        private SerializedProperty StartingValue;
        private int VariableTypeIndex;
        private string StartingUuid;

        public List<string> VariableStates;
        public List<string> StateUuids;

        private string NewOption;

        private void OnEnable()
        {
            dialogueOptionsVariable = (DialogueOptionsVariable)target;

            //Default Values
            VariableName = serializedObject.FindProperty("VariableName");
            VariableType = serializedObject.FindProperty("VariableType");

            //Numerical Type
            StartingValue = serializedObject.FindProperty("StartingValue");

            //Option Type
            VariableStates = dialogueOptionsVariable.VariableStates;
            StateUuids = dialogueOptionsVariable.StateUuids;
            StartingUuid = dialogueOptionsVariable.StartingUuid;
            if (VariableStates == null)
            {
                VariableStates = new List<string>();
                StateUuids = new List<string>();
            }
        }
        public override void OnInspectorGUI()
        {
            DSInspectorUtility.DrawHeader("Naming");
            VariableName.DrawPropertyField();
            VariableType.DrawPropertyField();

            switch (dialogueOptionsVariable.VariableType)
            {
                case VariableTypeEnum.Option:
                    {
                        DrawOptionArea();
                        dialogueOptionsVariable.StartingUuid = StartingUuid;

                        DrawDefaultOptionArea();
                        break;
                    }
                case VariableTypeEnum.Value:
                    {
                        DrawValueArea();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawValueArea()
        {
            DSInspectorUtility.DrawSpace();

            DSInspectorUtility.DrawHeader("Variable Value");

            StartingValue.DrawPropertyField();
        }

        private void DrawOptionArea()
        {
            DSInspectorUtility.DrawSpace();

            DSInspectorUtility.DrawHeader("Variable Options");

            List<string> uuidToRemove = new List<string>();
            List<string> optionToRemove = new List<string>();

            for (int idx = 0; idx < VariableStates.Count; idx++)
            {
                string current_uuid = StateUuids[idx];
                string current_value = VariableStates[idx];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X"))
                {
                    uuidToRemove.Add(current_uuid);
                    optionToRemove.Add(current_value);
                    continue;
                }
                string new_value = EditorGUILayout.TextField(current_value);
                EditorGUILayout.EndHorizontal();

                // If key has changed, update the dictionary
                if (new_value != current_value)
                {
                    if (!VariableStates.Contains(new_value))
                    {
                        VariableStates.Remove(current_value);
                        VariableStates.Insert(idx, new_value);
                    }
                }
            }
            foreach (string uuid in uuidToRemove)
            {
                StateUuids.Remove(uuid);
            }
            foreach (string value in optionToRemove)
            {
                VariableStates.Remove(value);
            }

            // Add new key-value pair
            DSInspectorUtility.DrawSpace();
            DSInspectorUtility.DrawHeader("Add New Option");
            EditorGUILayout.BeginHorizontal();
            NewOption = EditorGUILayout.TextField("New Option", NewOption);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add New"))
            {
                // Add new key-value pair to the dictionary
                if (!string.IsNullOrEmpty(NewOption))
                {
                    if (!VariableStates.Contains(NewOption))
                    {
                        VariableStates.Add(NewOption);
                        StateUuids.Add(Guid.NewGuid().ToString());
                    }
                }
            }
        }

        private void DrawDefaultOptionArea()
        {
            if (VariableStates.Count == 0)
            {
                return;
            }

            DSInspectorUtility.DrawSpace();

            DSInspectorUtility.DrawHeader("Default Variable");

            int popUpIdx = 0;
            if (StateUuids.Contains(StartingUuid))
            {
                popUpIdx = StateUuids.IndexOf(StartingUuid);
            }

            int newPopUpIdx = EditorGUILayout.Popup("Default Variable", popUpIdx, VariableStates.ToArray());

            StartingUuid = StateUuids[newPopUpIdx];
        }
    }
 }
