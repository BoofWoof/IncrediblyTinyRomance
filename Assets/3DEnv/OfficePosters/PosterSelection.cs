using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct OfficePoster
{
    public string Name;
    public string Artist;
    public string Description;
    public Sprite Image;
    public bool Unlocked;
}

public class PosterSelection : MonoBehaviour
{
    public RectTransform Content;
    public List<OfficePoster> Posters;

    public float TopBuffer = 20f;
    public float LeftBuffer = 20f;
    public float PortraitBuffer = 20f;

    public Vector2 Size = new Vector2(290, 400);

    public int Columns = 5;

    [Header("Preview")]
    public Image PreviewImage;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Artist;
    public TextMeshProUGUI Description;

    public OfficePoster SelectedPoster;

    public void Start()
    {
        SelectPoster(Posters[0]);

        CursorStateControl.AllowMouse(true);

        int spawned = 0;

        foreach (OfficePoster poster in Posters)
        {
            int col = spawned % Columns;
            int row = Mathf.FloorToInt(spawned/Columns);

            GameObject posterButton = new GameObject("Poster");

            RectTransform rect = posterButton.AddComponent<RectTransform>();
            rect.pivot = Vector2.up;
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;

            rect.sizeDelta = Size;
            rect.localScale = Vector2.one;

            rect.parent = Content;
            rect.anchoredPosition = new Vector2(
                LeftBuffer + (PortraitBuffer + Size.x)*col, 
                -TopBuffer + (PortraitBuffer + Size.y)*row
                );

            Image img = rect.AddComponent<Image>();
            img.sprite = poster.Image;

            Button butt = rect.AddComponent<Button>();

            butt.onClick.AddListener(() => SelectPoster(poster));

            spawned++;
        }
    }

    public void SelectPoster(OfficePoster poster)
    {
        SelectedPoster = poster;
        PreviewImage.sprite = SelectedPoster.Image;
        Title.text = SelectedPoster.Name;
        Artist.text = "Artist: " + SelectedPoster.Artist;
        Description.text = "Artist's Statement:" + "\"" + SelectedPoster.Description + "\"";

    }

    public void FinalizeSelection()
    {
        Destroy(gameObject);
    }


    public void OnDestroy()
    {
        transform.parent.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", SelectedPoster.Image.texture);
        CursorStateControl.AllowMouse(false);
    }
}
