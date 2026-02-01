using System.Collections.Generic;
using UnityEngine;

public class ArchiveDataHolderScript : MonoBehaviour
{
    public List<ArchiveDataSO> ArchiveDatas;

    public int Priority;

    public bool Submitted = false;
    public bool AutomaticallySubmitAtStart = false;

    public void Start()
    {
        if (AutomaticallySubmitAtStart) SubmitArchiveData();
    }

    public void SubmitArchiveData()
    {
        if (Submitted) return;
        Submitted = true;
        ArchiveScript.AddArchiveStatic(ArchiveDatas, Priority);
    }
}
