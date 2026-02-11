using UnityEngine;

[CreateAssetMenu(fileName = "PrayerThresholdAchievementSO", menuName = "Achievements/Prayer/PrayerThreshold")]
public class PrayerThresholdAchievementSO : AchievementAbstractSO
{
    public int TotalPrayerThreshold;
    public int GoodPrayerThreshold;
    public int BadPrayerThreshold;
    public override bool CheckCompletionCriteria()
    {
        return TotalPrayerThreshold <= PrayerScript.TotalPrayerCount && 
            GoodPrayerThreshold <= PrayerScript.GoodPrayerCount &&
            BadPrayerThreshold <= PrayerScript.BadPrayerCount;
    }

    public override string ProgressText()
    {
        string returnString = "";
        if (TotalPrayerThreshold > 0) returnString += $" ({PrayerScript.TotalPrayerCount}/{TotalPrayerThreshold})";
        if (GoodPrayerThreshold > 0) returnString += $" ({PrayerScript.GoodPrayerCount}/{GoodPrayerThreshold})";
        if (BadPrayerThreshold > 0) returnString += $" ({PrayerScript.BadPrayerCount}/{BadPrayerThreshold})";


        return returnString;
    }
}
