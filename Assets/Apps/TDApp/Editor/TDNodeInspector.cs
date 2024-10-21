using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDNodeScript))]
public class TDNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector elements

        TDNodeScript objectCreator = (TDNodeScript)target;

        if (GUILayout.Button("Create Object"))
        {
            objectCreator.CreateObject(); // Call the method to create the object
        }
    }
}