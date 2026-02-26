using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MascotButtonScript : MonoBehaviour
{
    public GameObject ButtonPrefab;

    public void SetChoices(ChoiceBlock choiceData)
    {
        Dictionary<string, string> choices = choiceData.ChoiceDictionary;

        foreach(KeyValuePair<string, string> pair in choices)
        {
            GameObject newButton = Instantiate(ButtonPrefab, gameObject.transform);
            newButton.transform.localScale = Vector3.one;
            newButton.transform.localRotation = Quaternion.identity;
            newButton.transform.localPosition = Vector3.zero;

            newButton.GetComponentInChildren<TMP_Text>().text = pair.Key;

            newButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SubmitButton(pair.Value);
            });
        }
    }

    public void SubmitButton(string response)
    {
        VisionMascotScript.OnSubmitChoice(response);
        Destroy(gameObject);
    }
}
