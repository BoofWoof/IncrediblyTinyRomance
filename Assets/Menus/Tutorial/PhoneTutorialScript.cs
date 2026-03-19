using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PhoneTutorialScript : MonoBehaviour
{
    private List<Transform> TutorialScreens;
    private int TutorialStep = 0;
    public bool CompletedTutorial = false;

    private void Start()
    {
        StartTutorial();
    }

    public void RestartTutorial()
    {
        StartTutorial();
    }

    public void HideTutorial()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
        GetComponent<Image>().enabled = false;
    }

    public void StartTutorial()
    {
        TutorialStep = 0;

        gameObject.SetActive(true);
        GetComponent<Image>().enabled = true;
        TutorialScreens = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            TutorialScreens.Add(child);
            child.gameObject.SetActive(false);
        }

        if (TutorialScreens.Count <= 0) return;
        TutorialScreens[0].gameObject.SetActive(true);

    }

    public void ProgressTutorial()
    {
        if (TutorialScreens.Count <= 0) return;
        TutorialScreens[TutorialStep].gameObject.SetActive(false);
        TutorialStep++;
        if(TutorialStep >= TutorialScreens.Count)
        {
            CompletedTutorial = true;
            GetComponent<Image>().enabled = false;
            return;
        }
        TutorialScreens[TutorialStep].gameObject.SetActive(true);
    }
}
