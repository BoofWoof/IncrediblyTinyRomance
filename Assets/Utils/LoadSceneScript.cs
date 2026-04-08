using UnityEngine;
using PixelCrushers;
using UnityEngine.UI;

public class LoadSceneScript : MonoBehaviour
{
    public Button LoadButton;

    public void Update()
    {
        if (LoadButton == null) return;
        LoadButton.interactable = SaveMenuScript.SaveFileExists();
    }

    public void LoadScene(string sceneName)
    {
        DaytaScript.ExternalSkipStart = false;
        Physics.gravity = Vector3.down * 9.8f;
        SaveSystem.LoadScene(sceneName);
    }
}
