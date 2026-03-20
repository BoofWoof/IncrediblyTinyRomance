using PixelCrushers;
using System;
using System.Collections.Generic;

public class VMascotSaver : Saver
{
    public VisionMascotScript targetScript;

    [Serializable]
    public class MascotSaveData
    {
        public List<MascotDifficultySaveData> DifficultyData = new List<MascotDifficultySaveData>();
        public string MascotName = "";

        public MascotSaveData FromMascot(VisionMascotScript mascotData)
        {
            DifficultyData = new List<MascotDifficultySaveData>();
            foreach (MascotDifficultyDialogueSO difficultyData in mascotData.DifficultyChangeMessages)
            {
                DifficultyData.Add(new MascotDifficultySaveData().FromDifficultyData(difficultyData));
            }
            MascotName = mascotData.MascotName;
            return this;
        }

        public void UnloadData(VisionMascotScript mascotData)
        {
            foreach(MascotDifficultySaveData mdData in DifficultyData)
            {
                foreach (MascotDifficultyDialogueSO difficultyData in mascotData.DifficultyChangeMessages)
                {
                    if(mdData.DifficultyName == difficultyData.DifficultyNames)
                    {
                        mdData.UnloadData(difficultyData);
                        continue;
                    }
                }
            }

            mascotData.MascotName = MascotName;
            mascotData.NameText.text = MascotName;

            mascotData.UpdateCharacter();
        }
    }
    [Serializable]
    public class MascotDifficultySaveData
    {
        public string DifficultyName;

        public int IncreaseOccurrences;
        public bool FirstIncrease;

        public int DecreaseOccurrences;
        public bool FirstDecrease;

        public int ClickOccurrences;

        public int SpamCloseOccurances;

        public List<CompletionEventSaveData> CompletionEvents = new List<CompletionEventSaveData>();

        public List<TimerEventSaveData> TimerEvents = new List<TimerEventSaveData>();

        public MascotDifficultySaveData FromDifficultyData(MascotDifficultyDialogueSO difficultyData)
        {
            DifficultyName = difficultyData.DifficultyNames;

            IncreaseOccurrences = difficultyData.IncreaseOccurrences;
            FirstIncrease = difficultyData.FirstIncrease;

            DecreaseOccurrences = difficultyData.DecreaseOccurrences;
            FirstDecrease = difficultyData.FirstDecrease;

            ClickOccurrences = difficultyData.ClickOccurrences;

            SpamCloseOccurances = difficultyData.SpamCloseOccurances;

            CompletionEvents = new List<CompletionEventSaveData>();
            foreach (VisionCompletionMascotText vcText in difficultyData.SetSolutionDialogues)
            {
                CompletionEvents.Add(new CompletionEventSaveData().FromCE(vcText));
            }

            TimerEvents = new List<TimerEventSaveData>();
            foreach (TimePassingDialogues tpText in difficultyData.TimeDialogues)
            {
                TimerEvents.Add(new TimerEventSaveData().FromTP(tpText));
            }

            return this;
        }

        public void UnloadData(MascotDifficultyDialogueSO difficultyData)
        {
            difficultyData.IncreaseOccurrences = IncreaseOccurrences;
            difficultyData.FirstIncrease = FirstIncrease;

            difficultyData.DecreaseOccurrences = DecreaseOccurrences;
            difficultyData.FirstDecrease = FirstDecrease;

            difficultyData.ClickOccurrences = ClickOccurrences;

            difficultyData.SpamCloseOccurances = SpamCloseOccurances;

            foreach (CompletionEventSaveData ceSD in CompletionEvents)
            {
                foreach (VisionCompletionMascotText vcText in difficultyData.SetSolutionDialogues)
                {
                    if(ceSD.CEName == vcText.CompletionName)
                    {
                        vcText.Triggered = ceSD.Triggered;
                        continue;
                    }
                }
            }

            foreach (TimerEventSaveData teSD in TimerEvents)
            {
                foreach (TimePassingDialogues tpText in difficultyData.TimeDialogues)
                {
                    if(tpText.TimeName == teSD.TEName)
                    {
                        tpText.TriggerOccurances = teSD.Occurances;
                        continue;
                    }
                }
            }
        }
    }
    [Serializable]
    public class CompletionEventSaveData
    {
        public string CEName;
        public bool Triggered;

        public CompletionEventSaveData FromCE(VisionCompletionMascotText completionData)
        {
            CEName = completionData.CompletionName;
            Triggered = completionData.Triggered;

            return this;
        }
    }
    [Serializable]
    public class TimerEventSaveData
    {
        public string TEName;
        public int Occurances;

        public TimerEventSaveData FromTP(TimePassingDialogues timeData)
        {
            TEName = timeData.TimeName;
            Occurances = timeData.TriggerOccurances;

            return this;
        }
    }

    public override string RecordData()
    {
        return SaveSystem.Serialize(new MascotSaveData().FromMascot(targetScript));
    }

    public override void ApplyData(string s)
    {
        MascotSaveData saveData = SaveSystem.Deserialize<MascotSaveData>(s);

        if (saveData == null) return;

        saveData.UnloadData(targetScript);
    }
}
