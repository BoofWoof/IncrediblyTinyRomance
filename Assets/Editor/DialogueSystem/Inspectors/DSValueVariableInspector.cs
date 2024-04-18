using DS.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace DS.DialogueVariables
{
    [CustomEditor(typeof(DialogueValueVariable))]
    public class DSValueVariableInspector : Editor
    {
        private SerializedProperty VariableName;
        private SerializedProperty StartingValue;

        private void OnEnable()
        {
            //Default Values
            VariableName = serializedObject.FindProperty("VariableName");
            StartingValue = serializedObject.FindProperty("StartingValue");

        }
        public override void OnInspectorGUI()
        {
            DSInspectorUtility.DrawHeader("Naming");
            VariableName.DrawPropertyField();
            DSInspectorUtility.DrawHeader("Values");
            StartingValue.DrawPropertyField();
        }
    }
 }
