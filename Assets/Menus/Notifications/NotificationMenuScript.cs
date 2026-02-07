using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationMenuScript : MonoBehaviour
{
    private static NotificationMenuScript instance;

    public GameObject NotificationPrefab;

    private static Dictionary<string, Sprite> Notifications = new Dictionary<string, Sprite>();

    public void Start()
    {
        instance = this;
    }

    public void GenerateNotifications()
    {
        ClearNotifications();

        foreach (KeyValuePair<string, Sprite> notificationData in Notifications)
        {
            GameObject newObject = Instantiate(NotificationPrefab, transform);
            newObject.transform.localScale = Vector3.one;
            newObject.transform.localRotation = Quaternion.identity;
            newObject.transform.localPosition = Vector3.zero;
            newObject.GetComponent<Image>().sprite = notificationData.Value;
        }
    }
    public void ClearNotifications()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public static void SetNotification(string SourceName, Sprite NotificationSprite)
    {
        if (Notifications.ContainsKey(SourceName)) return;

        Notifications.Add(SourceName, NotificationSprite);

        instance.GenerateNotifications();
    }
    public static void ReleaseNotification(string SourceName)
    {
        if (!Notifications.ContainsKey(SourceName)) return;

        Notifications.Remove(SourceName);

        instance.GenerateNotifications();
    }
}
