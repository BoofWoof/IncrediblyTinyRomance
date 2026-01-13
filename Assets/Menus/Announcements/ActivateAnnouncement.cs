using UnityEngine;

public class ActivateAnnouncement : MonoBehaviour
{

    public string AnnouncementText;

    public void TriggerAnnouncement()
    {
        AnnouncementScript.StartAnnouncement(AnnouncementText);
    }
}
