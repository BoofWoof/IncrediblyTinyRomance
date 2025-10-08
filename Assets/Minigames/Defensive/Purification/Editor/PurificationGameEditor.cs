using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PurificationGameScript))]
public class PurificationGameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the normal inspector first
        DrawDefaultInspector();

        PurificationGameScript purificationGameScript = (PurificationGameScript)target;

        // Add a button
        if (GUILayout.Button("UpdateRoutes"))
        {
            purificationGameScript.UpdatePipeRoutes();
        }
    }
}
