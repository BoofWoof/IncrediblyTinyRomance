using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CityScript))]
public class TeleportableCityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("ResetPosition (Edit Mode)"))
        {
            CityScript cityPositionScript = (CityScript)target;
            cityPositionScript.GoTo(cityPositionScript.CurrentNode);
        }
    }
}


[CustomEditor(typeof(OverworldPositionScript))]
public class TeleportableCharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("ResetPosition (Edit Mode)"))
        {
            OverworldPositionScript overworldPositionScript = (OverworldPositionScript)target;
            overworldPositionScript.GoTo(overworldPositionScript.CurrentNode);
        }
    }
}
