using UnityEngine;
using PixelCrushers;

public class LoadSceneScript : MonoBehaviour
{
    public void LoadScene(string SceneName)
    {
        SaveSystem.LoadScene(SceneName);
    }
}
