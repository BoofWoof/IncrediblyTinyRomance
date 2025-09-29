using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PipeStackScript))]
public class PipeStackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the normal inspector first
        DrawDefaultInspector();

        PipeStackScript pipeScript = (PipeStackScript)target;

        // Add a button
        if (GUILayout.Button("Update Pipe"))
        {
            pipeScript.SetPipeType(pipeScript.PipeTypeIdx);
            pipeScript.InstantRotation();
            SceneView.RepaintAll();
        }
    }
}
