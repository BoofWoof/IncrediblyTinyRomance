using PixelCrushers;
using System;
using System.Collections.Generic;
using System.Linq;
using static UpgradePanelSaver;

public class VisionsSaver : Saver
{
    [Serializable]
    public class VisionSaveData
    {
        public List<int> Difficulties = new List<int>();
        public List<int> PuzzlesCompleted = new List<int>();
        public List<float> TimeRecords = new List<float>();
        public int CurrentDifficutly;

        public VisionSaveData FromScript(TurkPuzzleScript puzzleScript)
        {
            CurrentDifficutly = TurkPuzzleScript.CurrentDifficutly;

            Difficulties = TurkPuzzleScript.TimeRecords.Keys.ToList();
            TimeRecords = TurkPuzzleScript.TimeRecords.Values.ToList();
            PuzzlesCompleted = TurkPuzzleScript.PuzzlesCompleted.Values.ToList();

            return this;
        }

        public void SetScript()
        {
            Dictionary<int, int> puzzlesComplete = new Dictionary<int, int>();
            Dictionary<int, float> timeRecords = new Dictionary<int, float>();
            for (int i = 0; i < Difficulties.Count; i++)
            {
                puzzlesComplete[Difficulties[i]] = PuzzlesCompleted[i];
                timeRecords[Difficulties[i]] = TimeRecords[i];
            }

            TurkPuzzleScript.CurrentDifficutly = CurrentDifficutly;
            TurkPuzzleScript.TimeRecords = timeRecords;
            TurkPuzzleScript.PuzzlesCompleted = puzzlesComplete;

            TurkPuzzleScript.instance.UpdateDifficultyButtons();
            TurkPuzzleScript.instance.GeneratePuzzle();

        }
    }

    public override string RecordData()
    {
        VisionSaveData newSaveData = new VisionSaveData().FromScript(TurkPuzzleScript.instance);
        return SaveSystem.Serialize(newSaveData);
    }

    public override void ApplyData(string s)
    {
        SaveSystem.Deserialize<VisionSaveData>(s).SetScript();
    }

}
