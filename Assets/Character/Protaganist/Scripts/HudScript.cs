using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public int LastTutorialQuestIndex = 3;
    public Coroutine TutorialCoroutine;

    public GameObject MessageNotification;

    public GameObject QuestPanel;
    public GameObject SubtitlePanel;

    public static HudScript instance;

    public static void ShowMessageNotification(bool show)
    {
        instance.MessageNotification.SetActive(show);
    }

    public static void SetQuestVisiblity(bool questVisible)
    {
        instance.QuestPanel.SetActive(questVisible);
    }
    public static void SetSubtitleVisiblity(bool subtitleVisible)
    {
        instance.SubtitlePanel.SetActive(subtitleVisible);
    }

    public static void SetReticleOpacity(float reticleOpacity)
    {
        instance.Reticle.GetComponent<Image>().color = new Color(1, 1, 1, reticleOpacity);
    }

    public void Awake()
    {
        instance = this;
        MessageNotification.SetActive(false);
    }

    public void Start()
    {
        PhonePositionScript.PhoneToggled += ShowReticle;

        if(DayInfo.CurrentDay == 1)
        {
            TutorialCoroutine = StartCoroutine(Tutorial());
        }
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

    public void SkipTutorial()
    {
        if(TutorialCoroutine!= null) StopCoroutine(TutorialCoroutine);

        ShutterButton.ObjectEnabled = true;

        if(MoveArrow != null)
        {
            Destroy(MoveArrow);
        }
        if (LookArrow != null)
        {
            Destroy(LookArrow);
        }
        if (InteractArrow != null)
        {
            Destroy(InteractArrow);
        }

        if (QuestManager.currentQuestIndex <= LastTutorialQuestIndex)
        {
            QuestManager.SetQuestByIndex(LastTutorialQuestIndex + 1);
        }
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

        QuestManager.CompleteQuest(QuestManager.currentQuest);

        while (!ConversationManagerScript.WaitingForEvent)
        {
            yield return null;
        }

        InteractArrow.SetActive(true);
        ShutterButton.ObjectEnabled = true;

        QuestManager.IncrementQuest();

        yield return StartCoroutine(WaitForContinue());

        ConversationManagerScript.instance.ForceNextDialogue();

        Destroy(InteractArrow);

        QuestManager.IncrementQuest();

        yield return StartCoroutine(WaitForContinue());

        QuestManager.IncrementQuest();
    }
    #endregion
}
