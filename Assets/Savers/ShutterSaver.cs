using PixelCrushers;
using UnityEngine;
using static PosterSaver;

public class ShutterSaver : Saver
{
    public class ShutterSave
    {
        public bool Open;
    }

    public override string RecordData()
    {
        ShutterSave newSaveData = new ShutterSave()
        {
            Open = !ShutterScript.ShuttersLowered
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        if (SaveSystem.Deserialize<ShutterSave>(s).Open)
        {
            ShutterScript.instance.InstantOpen();
        } else
        {
            ShutterScript.instance.InstantClose();
        }

        PhonePositionScript.AllowPhoneToggle = true;
        ShutterScript.instance.DestroyPhone();
        ShutterScript.instance.ShutterButton.ObjectEnabled = true;
    }
}
