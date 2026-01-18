using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public Image Panel;
    public GameObject Text1;
    public GameObject Text2;
    public GameObject Text3;
    public GameObject Buttons;
    public GameObject Explinations;

    public float FadePeriod;

    public void Awake()
    {
        ClearScreen();
    }
    public void StartGameOver()
    {
        gameObject.SetActive(true);
        StartCoroutine(GameoverCoroutine());
    }

    public void ClearScreen()
    {
        gameObject.SetActive(false);
        Panel.gameObject.SetActive(false);
        Panel.color = new Color(0, 0, 0, 0);
        Text1.SetActive(false);
        Text2.SetActive(false);
        Text3.SetActive(false);
        Buttons.SetActive(false);
        Explinations.SetActive(false);

    }

    IEnumerator GameoverCoroutine()
    {
        Debug.Log("GameoverStarted");

        Panel.gameObject.SetActive(true);

        float timePassed = 0f;

        while (timePassed < FadePeriod)
        {
            timePassed += Time.unscaledDeltaTime;
            Debug.Log(timePassed);
            Panel.color = new Color(0, 0, 0, timePassed/FadePeriod);
            yield return null;
        }
        Panel.color = new Color(0, 0, 0, 1);

        yield return new WaitForSecondsRealtime(2f);
        Text1.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        Text2.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        Text3.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        Buttons.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        Explinations.SetActive(true);
    }

    public void ResumeGame()
    {
        ClearScreen();
        CharacterSpeechScript.BroadcastForceGesture("A", "FeetUpIdle");
        OverworldBehavior.BroadcastBehaviors("A", "judge");
        CrossfadeScript.ResumeMusic();
        MusicSelectorScript.RevertOverworldSong();
    }
}
