using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


[ExecuteInEditMode]
public class TravelNodeTracker : MonoBehaviour
{
    public GameObject[] PositionNodes;
    
    public static TravelNodeTracker Instance;

    public GameObject DebugTarget;
    public int DebugNodeInt = 0;

    public void Start()
    {
        Instance = this;
    }

    public void TeleportTo()
    {
        Transform currentTransform = DebugTarget.transform;
        Transform newTransform = PositionNodes[DebugNodeInt].transform;

        currentTransform.position = newTransform.position;
        currentTransform.rotation = Quaternion.Euler(270f, newTransform.rotation.eulerAngles.z, newTransform.rotation.eulerAngles.y);
    }
    public void TeleportTo(int debugNodeInt)
    {
        Transform currentTransform = DebugTarget.transform;
        Transform newTransform = PositionNodes[debugNodeInt].transform;

        currentTransform.position = newTransform.position;
        currentTransform.rotation = Quaternion.Euler(270f, newTransform.rotation.eulerAngles.z, newTransform.rotation.eulerAngles.y);
    }
}


[CustomEditor(typeof(TravelNodeTracker))]
public class TeleportableCharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Teleport (Edit Mode)"))
        {
            TravelNodeTracker.Instance.TeleportTo();
        }
    }
}
