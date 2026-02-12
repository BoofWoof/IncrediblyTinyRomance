using System.Collections.Generic;
using UnityEngine;

public class PrayerTutorialScript : MonoBehaviour
{

    public int GoodPrayerCount = 0;
    public List<VoiceLineSO> GoodPrayerResponses;
    public int BadPrayerCount = 0;
    public List<VoiceLineSO> BadPrayerResponses;

    public void OnBadPrayer()
    {
        CharacterSpeechScript.BroadcastSpeechAttempt("RadioMilo", BadPrayerResponses[BadPrayerCount % BadPrayerResponses.Count]);
        BadPrayerCount++;
    }
    public void OnGoodPrayer()
    {
        if(GoodPrayerCount == 2)
        {
            Destroy(gameObject);
            return;
        }
        CharacterSpeechScript.BroadcastSpeechAttempt("RadioMilo", GoodPrayerResponses[GoodPrayerCount % GoodPrayerResponses.Count]);
        GoodPrayerCount++;
    }
}
