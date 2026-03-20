using PixelCrushers;
using Unity.VisualScripting;
using UnityEngine;
using static PosterSaver;

public class UpgradeHolderSaver : Saver
{
    [SerializeField]
    public class UpgradeHolderSaveData
    {
        public bool Submitted;
    }

    public override string RecordData()
    {
        UpgradeHolderSaveData newSaveData = new UpgradeHolderSaveData()
        {
            Submitted = GetComponent<UpgradeHolder>().Submitted
        };
        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        UpgradeHolderSaveData saveData = SaveSystem.Deserialize<UpgradeHolderSaveData>(s);

        if (saveData == null) return;

        if (saveData.Submitted) GetComponent<UpgradeHolder>().SubmitUpgrades();
    }
}
