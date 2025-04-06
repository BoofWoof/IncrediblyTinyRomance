using System.Collections;
using TMPro;
using UnityEngine;

public class PrayerFireworkTextScript : MonoBehaviour
{
    public float ExplosionDelay = 4.5f;
    public float MinScale = 0f;
    public float MaxScale = 5f;
    public float ScalePeriod = 3f;

    public TMP_Text TargetText;
    public Camera TextCamera;

    public AnimationCurve ScaleCurve;

    public RenderTexture TextRenderTexture;

    private Coroutine CurrentFirework;
    private ParticleSystem FireworkParticleSystem;

    public AudioSource FireworkExplosion;

    public void Start()
    {
        FireworkParticleSystem = GetComponent<ParticleSystem>();
    }

    public void ActivateFirework(string text)
    {
        if (CurrentFirework != null) { StopCoroutine(CurrentFirework); }
        CurrentFirework = StartCoroutine(FireworkCoroutine(text));
    }

    IEnumerator FireworkCoroutine(string text)
    {
        TextCamera.enabled = true;
        TargetText.text = text;

        yield return new WaitForSeconds(ExplosionDelay);

        FireworkExplosion.Play();

        Texture2D textTexture = ConvertRenderTextureToTexture2D(TextRenderTexture);
        Sprite textSprite = ConvertTexture2DToSprite(textTexture);
        ParticleSystem.ShapeModule fireworkShape = FireworkParticleSystem.shape;
        fireworkShape.sprite = textSprite;
        fireworkShape.texture = textTexture;

        FireworkParticleSystem.Play();
        TextCamera.enabled = false;
        transform.localScale = Vector3.one * MinScale;

        float timePassedSec = 0f;
        while (timePassedSec < ScalePeriod)
        {
            timePassedSec += Time.deltaTime;
            float progress = timePassedSec / ScalePeriod;
            float scale = (MaxScale - MinScale) * ScaleCurve.Evaluate(progress) + MinScale;
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        // Set the active RenderTexture
        RenderTexture.active = renderTexture;

        // Create a new Texture2D with the same dimensions
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // Read the pixels from the RenderTexture into the Texture2D
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        // Reset active RenderTexture
        RenderTexture.active = null;

        return texture2D;
    }

    Sprite ConvertTexture2DToSprite(Texture2D texture2D)
    {
        return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
    }
}
