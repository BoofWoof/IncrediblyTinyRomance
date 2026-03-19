using PixelCrushers;
using System;

public class PhoneTutorialSaver : Saver
{

    [Serializable]
    public class PhoneTutorialSaveData
    {
        public bool Completed = false;
    }

    public override string RecordData()
    {
        PhoneTutorialSaveData newSaveData = new PhoneTutorialSaveData()
        {
            Completed = GetComponent<PhoneTutorialScript>().CompletedTutorial
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        if (SaveSystem.Deserialize<PhoneTutorialSaveData>(s).Completed)
        {
            GetComponent<PhoneTutorialScript>().CompletedTutorial = true;
            GetComponent<PhoneTutorialScript>().HideTutorial();
        }
    }

}
