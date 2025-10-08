using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VentGridScript))]
public class VentGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the normal inspector first
        DrawDefaultInspector();

        VentGridScript ventGridScript = (VentGridScript)target;

        // Add a button
        if (GUILayout.Button("Spawn Grid"))
        {
            ventGridScript.SpawnGrid();
            SceneView.RepaintAll();
        }

        // Add a button
        if (GUILayout.Button("Save Grid"))
        {
        }

        // Add a button
        if (GUILayout.Button("Load Grid"))
        {
            SceneView.RepaintAll();
        }

        // Add a button
        if (GUILayout.Button("Clear"))
        {
            ventGridScript.ClearGrid();
        }
    }
}
