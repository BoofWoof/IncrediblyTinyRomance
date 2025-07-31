using UnityEditor;
using UnityEngine;

public class CityScript : MonoBehaviour
{
    public static CityScript Instance;

    public int CurrentNode;

    public void OnEnable()
    {
        Instance = this;
    }

    public void GoTo(int NodeIdx)
    {
        Transform EndTransform = CityNodeTracker.Instance.CityNodes[NodeIdx].transform;

        transform.position = EndTransform.position;
        transform.rotation = EndTransform.rotation;
    }
}

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
