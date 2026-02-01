using TMPro;
using UnityEngine;

public class ArchiveButtonScript : MonoBehaviour
{
    public ArchiveDataSO AssociatedArchiveData;

    public TMP_Text ButtonText;

    public void SetArchiveData(ArchiveDataSO ArchiveData)
    {
        AssociatedArchiveData = ArchiveData;
        ButtonText.text = ArchiveData.Title;
    }

    public void SubmitArchiveDataToShow()
    {
        ArchiveScript.SetDisplayedArchive(AssociatedArchiveData);
    }
}
