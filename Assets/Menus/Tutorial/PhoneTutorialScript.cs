using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhoneTutorialScript : MonoBehaviour
{
    private List<Transform> TutorialScreens;
    private int TutorialStep = 0;

    private void Start()
    {
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
        Destroy(TutorialScreens[TutorialStep].gameObject);
        TutorialStep++;
        if(TutorialStep >= TutorialScreens.Count)
        {
            Destroy(gameObject);
            return;
        }
        TutorialScreens[TutorialStep].gameObject.SetActive(true);
    }
}
