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

    public void AssignAchievementData(AchievementAbstractSO newData)
    {
        AchievementData = newData;

        ButtonText.text = AchievementData.ButtonText;
        TitleText.text = AchievementData.Title;
        ObjectiveText.text = "<b>Objective:</b> " + AchievementData.Objective;
        FlavorText.text = AchievementData.Flavor;

        SubmitAchievementButton.interactable = AchievementData.CheckCompletionCriteria();
    }

    public void SubmitData()
    {
        if(AchievementData.OnBuyAnnouncement.Length != 0) AnnouncementScript.StartAnnouncement(AchievementData.OnBuyAnnouncement);

        AchievementListScript.AddFinishedAchievement(AchievementData.Title);
        ActiveBroadcast.BroadcastActivation(AchievementData.ActivationData);

        transform.parent = null;
        AchievementListScript.CheckIfListIsEmpty();

        Destroy(gameObject);
    }
}
