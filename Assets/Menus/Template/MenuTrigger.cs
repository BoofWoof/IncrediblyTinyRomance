using UnityEngine;

public class MenuTrigger : MonoBehaviour
{
    private static int MenuOpenCount = 0;

    private static float NormalTimescale = 1f;
    private static bool ReticlePreviousShown = true;

    public void OnEnable()
    {
        if (MenuOpenCount == 0) PauseGame();
        MenuOpenCount++;

        Debug.Log("MenuCount: " + MenuOpenCount.ToString());
    }

    public void OnDisable()
    {
        if (MenuOpenCount == 0) return;
        MenuOpenCount--;
        if (MenuOpenCount <= 0) UnPauseGame();

        Debug.Log("MenuCount: " + MenuOpenCount.ToString());
    }

    public void UnPauseGame()
    {
        AudioListener.pause = false; // Mute audio when losing focus
        CursorStateControl.PauseMenuToggle(false);
        HudScript.instance.ShowReticle(!ReticlePreviousShown);
        Time.timeScale = NormalTimescale;
    }

    public void PauseGame()
    {
        AudioListener.pause = true; // Mute audio when losing focus
        CursorStateControl.PauseMenuToggle(true);
        ReticlePreviousShown = HudScript.instance.Reticle.activeInHierarchy;
        HudScript.instance.ShowReticle(false);
        NormalTimescale = Time.timeScale;
        Time.timeScale = 0;
    }
}