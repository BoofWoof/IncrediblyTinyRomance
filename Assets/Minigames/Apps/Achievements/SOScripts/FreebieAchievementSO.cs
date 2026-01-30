using UnityEngine;

[CreateAssetMenu(fileName = "FreebieAchievementSO", menuName = "Achievements/Misc/Freebie")]
public class FreebieAchievementSO : AchievementAbstractSO
{
    public override bool CheckCompletionCriteria()
    {
        return true;
    }
}
