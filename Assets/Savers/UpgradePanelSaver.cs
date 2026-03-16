using PixelCrushers;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelSaver : Saver
{
    public UpgradeScreenScript TargetScreen;

    [System.Serializable]
    public class UpgradePanelSaveData
    {
        public List<string> PreboughtUpgradeIDs = new List<string>();
    }

    public override string RecordData()
    {
        UpgradePanelSaveData newSaveData = new UpgradePanelSaveData()
        {
            PreboughtUpgradeIDs = new List<string>(TargetScreen.PreboughtUpgradeIDs)
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        TargetScreen.PreBuyUpgrades(SaveSystem.Deserialize<UpgradePanelSaveData>(s).PreboughtUpgradeIDs);
    }
}
