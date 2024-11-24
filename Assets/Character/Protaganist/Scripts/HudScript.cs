using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudScript : MonoBehaviour
{
    [Header("General Hud")]
    public GameObject Reticle;

    [Header("Tutorial Settings")]
    public GameObject MoveTutorial;
    public GameObject MoveArrow;
    public GameObject LookTutorial;
    public GameObject LookArrow;
    public GameObject InteractTutorial;
    public GameObject InteractArrow;
    public GameObject PhoneTutorial;
    public ActivatableObjectScript ShutterButton;

    private static bool ContinueTutorial = false;

    public void Start()
    {
        PhonePositionScript.PhoneToggled += ShowReticle;

        StartCoroutine(Tutorial());
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
        LookTutorial.SetActive(false);
        LookArrow.SetActive(false);

        InteractTutorial.SetActive(false);
        InteractArrow.SetActive(false);

        PhoneTutorial.SetActive(false);

        yield return StartCoroutine(WaitForContinue());

        Destroy(MoveArrow);
        LookTutorial.SetActive(true);
        LookArrow.SetActive(true);
        PlayerCam.EnableCameraMovement = true;

        yield return StartCoroutine(WaitForContinue());

        Destroy(LookArrow);
        InteractTutorial.SetActive(true);
        InteractArrow.SetActive(true);
        ShutterButton.ObjectEnabled = true;

        yield return StartCoroutine(WaitForContinue());

        Destroy(InteractArrow);
        PhoneTutorial.SetActive(true);

        yield return StartCoroutine(WaitForContinue());

        Destroy(MoveTutorial);
        Destroy(LookTutorial);
        Destroy(InteractTutorial);
        Destroy(PhoneTutorial);

    }
    #endregion
}
