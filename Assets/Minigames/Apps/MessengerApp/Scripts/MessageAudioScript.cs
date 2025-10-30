using UnityEngine;
using UnityEngine.UI;

public class MessageAudioScript : MonoBehaviour
{
    public AudioSource MessageAudioSource;
    public VoiceLineSO VoiceLine;

    public Sprite PlaySprite;
    public Sprite StopSprite;

    private bool PlayToggle = false;

    private Button AudioButton;

    void Start()
    {
        AudioButton = GetComponent<Button>();
        AudioButton.onClick.AddListener(OnButtonPressed);

        if (MessageAudioSource.isPlaying)
        {
            AudioButton.GetComponent<Image>().sprite = StopSprite;
            PlayToggle = true;
        }
    }

    private void Update()
    {
        if (!MessageAudioSource.isPlaying)
        {
            AudioButton.GetComponent<Image>().sprite = PlaySprite;
            PlayToggle = false;
        }
    }

    void OnButtonPressed()
    {
        PlayToggle = !PlayToggle;
        if (PlayToggle)
        {
            AudioButton.GetComponent<Image>().sprite = StopSprite;
            MessageAudioSource.clip = VoiceLine.AudioData;
            MessageAudioSource.Play();
        } else
        {
            AudioButton.GetComponent<Image>().sprite = PlaySprite;
            MessageAudioSource.Stop();
        }
    }
}
