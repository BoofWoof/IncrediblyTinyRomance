using PixelCrushers;
using System;
using UnityEngine;

public class EndOfDaySaver : Saver
{
    public EndOfDayScript targetScript;

    [Serializable]
    public class EndOfDaySaveData
    {
        public float SavedTime;
        public float StartingMiloLike;
        public float StartingAriesLike;
        public float StartingAriesDislike;
        public float TotalVentTime;

        public EndOfDaySaveData FromEndOfDay(EndOfDayScript eofData)
        {
            SavedTime = (Time.time - eofData.StartingTime) + eofData.SavedTime;
            StartingMiloLike = eofData.StartingMiloLike;
            StartingAriesLike = eofData.StartingAriesLike;
            StartingAriesDislike = eofData.StartingAriesDislike;
            TotalVentTime = PurificationGameScript.TotalTime;

            return this;
        }

        public void LoadData(EndOfDayScript eofData)
        {
            eofData.SavedTime = SavedTime;
            eofData.StartingMiloLike = StartingMiloLike;
            eofData.StartingAriesLike = StartingAriesLike;
            eofData.StartingAriesDislike = StartingAriesDislike;
            PurificationGameScript.TotalTime = TotalVentTime;
        }
    }

    public override string RecordData()
    {
        EndOfDaySaveData newSaveData = new EndOfDaySaveData().FromEndOfDay(targetScript);
        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        EndOfDaySaveData saveData = SaveSystem.Deserialize<EndOfDaySaveData>(s);

        if (saveData == null) return;

        saveData.LoadData(targetScript);
    }
}
