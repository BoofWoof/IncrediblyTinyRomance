using PixelCrushers;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class RotationSaver : Saver
{
    public override string RecordData()
    {
        Quaternion rot = transform.rotation;
        return $"{rot.x},{rot.y},{rot.z},{rot.w}";
    }

    public override void ApplyData(string data)
    {

        if (string.IsNullOrEmpty(data)) return;

        string[] rotParts = data.Split(',');

        if (rotParts.Length == 4 &&
            float.TryParse(rotParts[0], out float x) &&
            float.TryParse(rotParts[1], out float y) &&
            float.TryParse(rotParts[2], out float z) &&
            float.TryParse(rotParts[3], out float w))
        {
            Quaternion rot = new Quaternion(x, y, z, w);
            transform.rotation = rot;
            Debug.Log("Loaded Rotation:" + rot.eulerAngles);
        }
    }
}
