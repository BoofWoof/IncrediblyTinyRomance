using PixelCrushers;
using System.Collections.Generic;
using UnityEngine;

public class PosterSaver : Saver
{
    [System.Serializable]
    public class PosterSaveData
    {
        public List<string> PosterData = new List<string>();
    }

    public override string RecordData()
    {
        PosterSaveData newPosterSaveData = new PosterSaveData()
        {
            PosterData = UnlockablesManager.UnlockedPostersList
        };
        return SaveSystem.Serialize(newPosterSaveData);
    }

    public override void ApplyData(string s)
    {
        PosterSaveData saveData = SaveSystem.Deserialize<PosterSaveData>(s);

        if (saveData == null) return;

        UnlockablesManager.LoadFromList(saveData.PosterData);
    }
}
