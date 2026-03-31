using UnityEngine;
using PixelCrushers;
using UnityEngine.UI;

public class LoadSceneScript : MonoBehaviour
{
    public Button LoadButton;

    public void Update()
    {
        LoadButton.interactable = SaveMenuScript.SaveFileExists();
    }
}
