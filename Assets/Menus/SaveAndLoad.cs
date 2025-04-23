using PixelCrushers;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    public void SaveGame()
    {
        SaveSystem.SaveToSlot(0);
    }

    public void LoadGame()
    {
        SaveSystem.LoadFromSlot(0);
    }
}
