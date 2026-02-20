using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MessageSendScript : MonoBehaviour
{
    public float SendPeriodSec = 1f;
    public float FinalMessageSize = 0.1f;

    public Button SubmissionButton;
    public TMP_Text AuthorText;

    public GameObject SpecialReply;
    public GameObject RageBaitReply;

    public bool IsSpecial = false;

    Vector3 backupPos;

    public bool SystemEnable = false;
    public bool PhoneEnabled = false;
    public TMP_Text MessageText;

    private Color OriginalColor = Color.white;

    public void OnPhoneToggle(bool PhoneRaised)
    {
        PhoneEnabled = PhoneRaised;
        UpdateInteractability();
    }

    public void Awake()
    {
        OriginalColor = SubmissionButton.GetComponentInChildren<TMP_Text>().color;
    }

    public void Start()
    {

        SpecialReply?.SetActive(false);
        RageBaitReply?.SetActive(false);

        backupPos = transform.localPosition;

        IsSpecial = false;

        OriginalColor = MessageText.color;

        PhonePositionScript.PhoneToggled += OnPhoneToggle;
    }

    public void SetAuthorName(string newName)
    {
        AuthorText.text = newName;
    }

    public void SetButtonText(string newText)
    {
        SubmissionButton.GetComponentInChildren<TMP_Text>().text = newText;
    }

    public void SetNormalResponse()
    {
        SpecialReply.SetActive(false);
        RageBaitReply?.SetActive(false);
        IsSpecial = false;
    }

    public void SetSpecialResponse(bool goodReply)
    {
        if(goodReply) SpecialReply.SetActive(true);
        else RageBaitReply.SetActive(true);
        IsSpecial = true;
    }

    public void RestartMessage()
    {
        SystemEnable = true;
        UpdateInteractability();
        transform.localPosition = backupPos;
        transform.localScale = Vector3.one;
    }

    public void ActivateSend()
    {
        StartCoroutine(SendMessage());
    }

    public void DisableButton()
    {
        SubmissionButton.interactable = false;
    }

    public void EnableButton()
    {
        SubmissionButton.interactable = true;
    }

    private void UpdateInteractability()
    {
        bool ButtonEnabled = SystemEnable && !PhoneEnabled;
        SubmissionButton.interactable = ButtonEnabled;

        if(ButtonEnabled) MessageText.color = OriginalColor;
        else MessageText.color = Color.clear;

        AuthorText.gameObject.SetActive(ButtonEnabled);
    }

    IEnumerator SendMessage()
    {
        float timePassed = 0f;
        SystemEnable = false;
        UpdateInteractability();

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
