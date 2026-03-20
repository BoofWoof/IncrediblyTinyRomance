using UnityEngine;
using PixelCrushers;
using UnityEngine.UI;

public class LoadSceneScript : MonoBehaviour
{
    public Button LoadButton;

    public void LoadScene(string SceneName)
    {
        SaveSystem.LoadScene(SceneName);
    }

    public void Update()
    {
        LoadButton.interactable = SaveSystem.HasSavedGameInSlot(3);
    }

    public void LoadGame(int SlotIdx)
    {
        if (!SaveSystem.HasSavedGameInSlot(SlotIdx))
        {
            for (int i = 0; i < 10; i++) // Check first 10 slots
            {
                if (SaveSystem.HasSavedGameInSlot(i))
                {
                    Debug.Log("Save exists in slot: " + i);
                }
            }
        }
        Physics.gravity = Vector3.down * 9.8f;
        SaveSystem.LoadFromSlot(SlotIdx);
    }
}
