using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MascotTextFieldScript : MonoBehaviour
{
    public Button SubmitButton;
    public TMP_Text RemainingCharactersText;
    public TMP_InputField TextInputField;

    private string CurrentText = "";
    private int MinCharacterLength = 3;
    private int MaxCharacterLength = 6;

    public UnityEvent<string> OnTextSubmission;

    public void SetTextLimit(int minCharacterLength, int maxCharacterLength)
    {
        MaxCharacterLength = maxCharacterLength;
        MinCharacterLength = minCharacterLength;

        TextInputField.characterLimit = maxCharacterLength;
    }

    public void OnSubmit()
    {
        if(CurrentText.Length == 0)
        {
            OnTextSubmission?.Invoke(TextInputField.text);
        } else
        {
            OnTextSubmission?.Invoke(CurrentText);
        }

        Destroy(gameObject);
    }

    public void OnTextChange(string newText)
    {
        CurrentText = newText;
        CheckValidCharacterLength();
    }

    public void CheckValidCharacterLength()
    {
        SubmitButton.interactable = true;
        RemainingCharactersText.color = Color.white;

        int remainingCharacters = MaxCharacterLength - CurrentText.Length;

        RemainingCharactersText.text = remainingCharacters.ToString();

        if(CurrentText.Length > MaxCharacterLength)
        {
            RemainingCharactersText.text = " Shorten Name";
            RemainingCharactersText.color = Color.red;
            SubmitButton.interactable = false;
        }
        if (CurrentText.Length < MinCharacterLength)
        {
            RemainingCharactersText.text = " Lengthen Name";
            RemainingCharactersText.color = Color.red;
            SubmitButton.interactable = false;
        }

    }
}
