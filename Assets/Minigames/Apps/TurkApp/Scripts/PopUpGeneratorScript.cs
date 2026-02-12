using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpGeneratorScript : MonoBehaviour
{
    public List<Sprite> PopupSprites;

    public float ChanceToShow = 0.2f;
    public AudioSource ShowNoise;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        GetComponent<Image>().sprite = PopupSprites[Random.Range(0, PopupSprites.Count)];
        ShowNoise.Play();
        transform.localPosition = new Vector3(Random.Range(-350, 350), Random.Range(-150, 150), 0);
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }

    public void MaybeShowPopup()
    {
        gameObject.SetActive(Random.value <= ChanceToShow && TurkPuzzleScript.CurrentDifficutly > 0);
    }
}
