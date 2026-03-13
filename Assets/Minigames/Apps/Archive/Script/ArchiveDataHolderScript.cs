using PixelCrushers;
using System;
using System.Collections.Generic;

public class ArchiveDataHolderScript : Saver
{
    public List<ArchiveDataSO> ArchiveDatas;

    public int Priority;

    public bool Submitted = false;
    public bool AutomaticallySubmitAtStart = false;

    public string UnlockAnnouncement;
    
    [Serializable]
    public class ArchiveDiscoveredSaveData
    {
        public bool Saved = false;
    }

    public override void ApplyData(string s)
    {
        if(SaveSystem.Deserialize<ArchiveDiscoveredSaveData>(s).Saved) SubmitArchiveDataWithAnnouncement(false);
    }

    public override string RecordData()
    {
        ArchiveDiscoveredSaveData newSaveData = new ArchiveDiscoveredSaveData()
        {
            Saved = Submitted
        };
        return SaveSystem.Serialize(newSaveData);
    }

    override public void Start()
    {
        base.Start();
        if (AutomaticallySubmitAtStart) SubmitArchiveData();
    }

    public void SubmitArchiveData()
    {
        SubmitArchiveDataWithAnnouncement(true);
    }

    public void SubmitArchiveDataWithAnnouncement(bool sendAnnouncement = true)
    {
        if (Submitted) return;
        Submitted = true;
        ArchiveScript.AddArchiveStatic(ArchiveDatas, Priority);

        if (UnlockAnnouncement.Length > 0 && sendAnnouncement) AnnouncementScript.StartAnnouncement(UnlockAnnouncement);

        if (!AutomaticallySubmitAtStart) ArchiveScript.instance.ShowNotification();
    }
}
