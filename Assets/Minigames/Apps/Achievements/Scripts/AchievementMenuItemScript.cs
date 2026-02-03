using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementMenuItemScript : MonoBehaviour
{
    public Button SubmitAchievementButton;

    public TMP_Text ButtonText;
    public TMP_Text TitleText;
    public TMP_Text ObjectiveText;
    public TMP_Text FlavorText;

    public AchievementAbstractSO AchievementData;

    public Transform BasePanel;
    public float SwipePeriod = 1f;
    public float FinalSwipeXValue = 2000f;

    public float ShutterPeriod = 0.2f;

    public void AssignAchievementData(AchievementAbstractSO newData)
    {
        AchievementData = newData;

        ButtonText.text = AchievementData.ButtonText;
        TitleText.text = AchievementData.Title;
        ObjectiveText.text = "<color=white><b>Objective:</b></color> " + AchievementData.Objective;
        FlavorText.text = AchievementData.Flavor;

        SubmitAchievementButton.interactable = AchievementData.CheckCompletionCriteria();
    }

    public void SubmitData()
    {
        if(AchievementData.OnBuyAnnouncement.Length != 0) AnnouncementScript.StartAnnouncement(AchievementData.OnBuyAnnouncement);

        AchievementListScript.AddFinishedAchievement(AchievementData.Title);
        ActiveBroadcast.BroadcastActivation(AchievementData.ActivationData);

        StartCoroutine(SwipePanel());
    }

    public IEnumerator SwipePanel()
    {
        float timePassed = 0f;
        float startingXValue = BasePanel.localPosition.x;

        while(timePassed < SwipePeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / SwipePeriod;
            {
                BasePanel.localPosition = new Vector3(Mathf.Lerp(startingXValue, FinalSwipeXValue, progress), 0, 0);
            }
            yield return null;
        }

        timePassed = 0f;
        RectTransform targetTransforms = GetComponent<RectTransform>();
        Vector2 startingSize = targetTransforms.sizeDelta;
        while (timePassed < ShutterPeriod)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / ShutterPeriod;
            targetTransforms.sizeDelta = new Vector2(startingSize.x, Mathf.Lerp(startingSize.y, 0, progress));
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
            yield return null;
        }

        transform.parent = null;
        AchievementListScript.CheckIfListIsEmpty();
        Destroy(gameObject);
    }
}
