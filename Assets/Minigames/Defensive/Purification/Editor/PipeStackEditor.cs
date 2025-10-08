using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PipeStackScript))]
public class PipeStackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PipeStackScript pipeScript = (PipeStackScript)target;

        EditorGUILayout.LabelField("SetSourceSinkType", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Set Normal"))
        {
            pipeScript.SetNormal();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Set Cap"))
        {
            pipeScript.SetCap();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Set Source"))
        {
            pipeScript.SetSource();
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Set Sink"))
        {
            pipeScript.SetSink();
            SceneView.RepaintAll();
        }

        EditorGUILayout.LabelField("SetBlockType", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Set Empty"))
        {
            pipeScript.SetPipeType(0);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Set +"))
        {
            pipeScript.SetPipeType(1);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Set -|"))
        {
            pipeScript.SetPipeType(2);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Set Double L"))
        {
            pipeScript.SetPipeType(3);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Set L"))
        {
            pipeScript.SetPipeType(4);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Set Straight"))
        {
            pipeScript.SetPipeType(5);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Set T"))
        {
            pipeScript.SetPipeType(6);
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Set Stub"))
        {
            pipeScript.SetPipeType(7);
            SceneView.RepaintAll();
        }

        EditorGUILayout.LabelField("RotateBlock", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Rotate CW"))
        {
            pipeScript.SetRotationTracker(pipeScript.RotationTracker += 1);
            pipeScript.InstantRotation();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Rotate CC"))
        {
            pipeScript.SetRotationTracker(pipeScript.RotationTracker -= 1);
            pipeScript.InstantRotation();
            SceneView.RepaintAll();
        }

        // Draw the normal inspector first
        DrawDefaultInspector();

        // Add a button
        if (GUILayout.Button("Update Pipe"))
        {
            pipeScript.SetPipeType(pipeScript.PipeTypeIdx);
            pipeScript.InstantRotation();
            SceneView.RepaintAll();
        }
    }
}
