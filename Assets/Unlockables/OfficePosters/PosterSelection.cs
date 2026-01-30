using JetBrains.Annotations;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;


public class PosterSelection : MonoBehaviour
{
    public RectTransform Content;

    public float TopBuffer = 20f;
    public float LeftBuffer = 20f;
    public float PortraitBuffer = 20f;

    public Vector2 Size = new Vector2(290, 400);

    public int Columns = 4;

    [Header("Preview")]
    public Image PreviewImage;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Artist;
    public TextMeshProUGUI Description;

    public OfficePoster SelectedPoster;

    public Material PosterMaterial;

    public List<GameObject> Posters = new List<GameObject>();

    public void Update()
    {
        Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
    }

    public void Start()
    {
        int rows = Mathf.CeilToInt(UnlockablesManager.PostersList.Count / Columns) + 1;

        Content.sizeDelta = new Vector2(Content.sizeDelta.x, 2f * TopBuffer + (PortraitBuffer + Size.y) * rows);

        SelectPoster(UnlockablesManager.PostersList[0]);

        CursorStateControl.AllowMouse(true);

        int spawned = 0;

        foreach (OfficePoster poster in UnlockablesManager.PostersList)
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
            rect.localPosition = Vector2.zero;

            Image img = rect.AddComponent<Image>();
            img.sprite = poster.Image;

            if (!poster.Unlocked)
            {
                img.material = PosterMaterial;
            } else
            {

                Button butt = rect.AddComponent<Button>();

                butt.onClick.AddListener(() => SelectPoster(poster));
            }

            spawned++;
            Posters.Add(posterButton);
        }
    }

    public void ExitMenu()
    {
        SelectedPoster = null;
        Destroy(gameObject);
    }

    public void SelectPoster(OfficePoster poster)
    {
        SelectedPoster = poster;
        PreviewImage.sprite = SelectedPoster.Image;
        Title.text = SelectedPoster.Name;
        Artist.text = "Artist: " + SelectedPoster.Artist;
        Description.text = "Artist's Statement: " + "\"" + SelectedPoster.Description + "\"";

    }

    public void FinalizeSelection()
    {
        Destroy(gameObject);
    }


    public void OnDestroy()
    {
        if(SelectedPoster != null) transform.parent.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", SelectedPoster.Image.texture);
        CursorStateControl.AllowMouse(false);
    }
}
