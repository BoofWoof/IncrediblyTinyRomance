using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MessageOptionsScript : MonoBehaviour
{
    [Header("Objects")]
    public GameObject button_template;

    [Header("Data")]
    public List<string> options = new List<string>();

    [Header("Tuning")]
    public int border_width = 25;
    public int border_height = 15;
    public int minimum_width = 200;
    public int maximum_width = 750;

    [HideInInspector]
    public float height = 0;

    private float line_height = 500;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (string option in options)
        {
            CreateButton(option);
        }
    }

    private void CreateButton(string text)
    {
        GameObject new_button = Instantiate(button_template, transform);
        new_button.transform.localPosition = new Vector2(0, -height);

        RectTransform button_back = new_button.GetComponent<RectTransform>();
        TextMeshProUGUI button_text = new_button.GetComponentInChildren<TextMeshProUGUI>();
        RectTransform text_rect = button_text.gameObject.GetComponent<RectTransform>();

        text_rect.sizeDelta = new Vector2(maximum_width, line_height);
        text_rect.localPosition = new Vector2(-border_width, -border_height);

        button_text.text = text;
        button_text.ForceMeshUpdate();
        float text_width = button_text.GetRenderedValues().x;
        float text_height = button_text.GetRenderedValues().y;

        text_rect.sizeDelta = new Vector2(text_width, text_height);
        button_back.sizeDelta = new Vector2(text_width + border_width*2, text_height + border_height*2);
        height += text_height + border_height * 2;
    }
}
