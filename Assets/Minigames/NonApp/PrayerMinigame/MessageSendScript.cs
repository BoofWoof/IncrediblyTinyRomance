using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MessageSendScript : MonoBehaviour
{
    public float SendPeriodSec = 1f;
    public float FinalMessageSize = 0.1f;

    Vector3 backupPos;

    public void Start()
    {
        backupPos = transform.localPosition;
    }

    public void RestartMessage()
    {
        GetComponent<Button>().interactable = true;
        transform.localPosition = backupPos;
        transform.localScale = Vector3.one;
    }

    public void ActivateSend()
    {
        StartCoroutine(SendMessage());
    }

    IEnumerator SendMessage()
    {
        float timePassed = 0f;
        GetComponent<Button>().interactable = false;

        while (timePassed < SendPeriodSec)
        {
            timePassed += Time.deltaTime;
            float percentPassed = timePassed / SendPeriodSec;
            float newScale = Mathf.Lerp(1, FinalMessageSize, percentPassed);
            transform.localScale = Vector3.one*newScale;

            float verticalPos = Mathf.Lerp(backupPos.y, -650, percentPassed);
            transform.localPosition = new Vector3(transform.localPosition.x, verticalPos, transform.localPosition.z);

            yield return null;
        }
    }
}
