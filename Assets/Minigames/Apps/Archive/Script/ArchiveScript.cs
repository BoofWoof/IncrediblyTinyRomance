using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ArchivePriorityData
{
    public int Priority;
    public ArchiveDataSO Data;
}

public class ArchiveScript : MonoBehaviour
{
    public static ArchiveScript instance;

    public List<RectTransform> ContentSizeFitterObjects;

    public ScrollRect TargetScrollRect;

    public GameObject TextBoxObject;
    public GameObject SelectAnArchiveText;

    public GameObject ArchiveButtonPrefab;

    public Transform TargetArchiveSelectorTransform;
    public TMP_Text SourceText;
    public TMP_Text DocumentText;

    public AudioSource ArchiveClick;

    public List<ArchivePriorityData> SortedData = new List<ArchivePriorityData>();

    public void OnEnable()
    {
        instance = this;
        ResetArchive();
    }

    public void ResetArchive()
    {
        SourceText.text = "Cite Your Sources";
        DocumentText.text = "";
        SelectAnArchiveText.SetActive(true);
        TextBoxObject.SetActive(false);
    }
    public void UpdateList()
    {
        ClearList();
        foreach (ArchivePriorityData archivePriorityData in SortedData)
        {
            GameObject newButton = Instantiate(ArchiveButtonPrefab, TargetArchiveSelectorTransform);
            newButton.transform.localRotation = Quaternion.identity;
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localPosition = Vector3.zero;

            newButton.GetComponent<ArchiveButtonScript>().SetArchiveData(archivePriorityData.Data);
        }
    }

    public static void SetDisplayedArchive(ArchiveDataSO archiveData)
    {
        instance.ArchiveClick.Play();

        instance.TextBoxObject.SetActive(true);
        instance.SourceText.text = "FROM: " + archiveData.Source;
        instance.DocumentText.text = archiveData.DocumentData.text;
        instance.SelectAnArchiveText.SetActive(false);

        instance.UpdateContentFitters();

        instance.TargetScrollRect.verticalNormalizedPosition = 1f;
    }

    public void UpdateContentFitters()
    {
        foreach (RectTransform contentSizeFitterObject in ContentSizeFitterObjects)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                contentSizeFitterObject
            );
        }
    }

    public void ClearList()
    {
        foreach (Transform child in TargetArchiveSelectorTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public static void AddArchiveStatic(List<ArchiveDataSO> archiveDatas, int priority)
    {
        instance.AddArchive(AddPriotiyToArchive(archiveDatas, priority));
    }

    private void AddArchive(List<ArchivePriorityData> archiveDatas)
    {
        int insertIdx = 0;

        foreach (ArchivePriorityData pArchive in SortedData)
        {
            if (pArchive.Priority > archiveDatas[0].Priority) break;
            insertIdx++;
        }
        SortedData.InsertRange(insertIdx, archiveDatas);

        UpdateList();
    }

    private static List<ArchivePriorityData> AddPriotiyToArchive(List<ArchiveDataSO> archiveDatas, int priority)
    {
        List<ArchivePriorityData> newPArchiveList = new List<ArchivePriorityData>();

        foreach (ArchiveDataSO archiveData in archiveDatas)
        {
            newPArchiveList.Add(new ArchivePriorityData
            {
                Data = archiveData,
                Priority = priority
            });
        }

        return newPArchiveList;
    }
}
