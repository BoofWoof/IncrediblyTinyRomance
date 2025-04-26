using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudScript : MonoBehaviour
{
    [Header("General Hud")]
    public GameObject Reticle;

    [Header("Tutorial Settings")]
    public GameObject MoveArrow;
    public GameObject LookArrow;
    public GameObject InteractArrow;
    public ActivatableObjectScript ShutterButton;

    private static bool ContinueTutorial = false;

    public void Start()
    {
        PhonePositionScript.PhoneToggled += ShowReticle;

        StartCoroutine(Tutorial());
    }

    public void OnDisable()
    {
        PhonePositionScript.PhoneToggled -= ShowReticle;
    }

    public void ShowReticle(bool show)
    {
        Reticle.SetActive(!show);
    }

    #region Tutorial
    public static void SetContinueTutorial()
    {
        ContinueTutorial = true;
    }
    public IEnumerator WaitForContinue()
    {
        while(!ContinueTutorial) yield return null;
        ContinueTutorial = false;
    }
    public IEnumerator Tutorial()
    {
        LookArrow.SetActive(false);

        InteractArrow.SetActive(false);

        yield return StartCoroutine(WaitForContinue());

        Destroy(MoveArrow);
        LookArrow.SetActive(true);

        QuestManager.IncrementQuest();

        yield return StartCoroutine(WaitForContinue());

        Destroy(LookArrow);
        InteractArrow.SetActive(true);
        ShutterButton.ObjectEnabled = true;

        QuestManager.IncrementQuest();

        yield return StartCoroutine(WaitForContinue());

        Destroy(InteractArrow);

        QuestManager.IncrementQuest();

        yield return StartCoroutine(WaitForContinue());

        QuestManager.IncrementQuest();
    }
    #endregion
}
