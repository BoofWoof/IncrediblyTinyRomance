using TMPro;
using UnityEngine;

public class ArchiveButtonScript : MonoBehaviour
{
    public ArchiveDataSO AssociatedArchiveData;

    public TMP_Text ButtonText;

    private string Title;

    public void SetArchiveData(ArchiveDataSO ArchiveData, bool newDocument)
    {
        AssociatedArchiveData = ArchiveData;
        ButtonText.text = ArchiveData.Title;
        Title = ArchiveData.Title;

        if (newDocument) ButtonText.text = $"<color=red>NEW</color> {ButtonText.text}";
    }

    public void SubmitArchiveDataToShow()
    {
        ArchiveScript.SetDisplayedArchive(AssociatedArchiveData);
        ArchiveScript.AddDcoumentToRead(Title);
        ButtonText.text = Title;
    }
}
