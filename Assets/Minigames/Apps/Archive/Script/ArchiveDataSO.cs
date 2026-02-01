using UnityEngine;

[CreateAssetMenu(fileName = "ArchiveDataSO", menuName = "ArchiveDataSO")]
public class ArchiveDataSO : ScriptableObject
{
    public string Title;
    public string Source;
    public TextAsset DocumentData;
}
