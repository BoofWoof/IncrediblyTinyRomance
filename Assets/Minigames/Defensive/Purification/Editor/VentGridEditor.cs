using PixelCrushers.DialogueSystem.ChatMapper;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
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
            PurificationLevelSO saveData = new PurificationLevelSO();
            saveData.LevelName = ventGridScript.LevelName;
            saveData.LevelDescription = ventGridScript.LevelDescription;
            saveData.OnScreenDataPos = ventGridScript.transform.localPosition;
            saveData.SquareSize = ventGridScript.TileSize;

            saveData.ConnectedCutsceneName = "";
            saveData.OnScreenData = null;

            saveData.Rows = ventGridScript.Rows;
            saveData.Cols = ventGridScript.Cols;

            saveData.Data = new List<PipeData>();
            foreach (GameObject pipeStack in ventGridScript.PipeStacks)
            {
                PipeStackScript pipeStackScript = pipeStack.GetComponent<PipeStackScript>();
                PipeData newPipeData = new PipeData();

                newPipeData.PipeTypeID = pipeStackScript.PipeTypeIdx;
                newPipeData.Rotations = pipeStackScript.RotationTracker;

                newPipeData.isCap = pipeStackScript.isCapped;
                newPipeData.isSource = pipeStackScript.isSource;
                newPipeData.isSink = pipeStackScript.isGoal;

                newPipeData.canRotate = pipeStackScript.canRotate;

                saveData.Data.Add(newPipeData);
            }
            AssetDatabase.CreateAsset(saveData, "Assets/Minigames/Defensive/Purification/Levels/" + saveData.LevelName + ".asset");
        }

        // Add a button
        if (GUILayout.Button("Load Grid"))
        {
            PurificationLevelSO saveData = AssetDatabase.LoadAssetAtPath<PurificationLevelSO>("Assets/Minigames/Defensive/Purification/Levels/" + ventGridScript.LevelName + ".asset");

            if (saveData == null)
            {
                Debug.Log("No level at this path name.");
                return;
            }

            ventGridScript.SpawnGridFromSaveData(saveData);
        }

        // Add a button
        if (GUILayout.Button("Clear"))
        {
            ventGridScript.ClearGrid();
        }
    }
}
