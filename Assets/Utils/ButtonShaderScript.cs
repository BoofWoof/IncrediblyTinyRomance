using UnityEngine;
using UnityEngine.UI;

public class ButtonShaderScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        Image imgComponent = GetComponent<Image>();
        imgComponent.material.SetColor("_Tint", imgComponent.color);
        imgComponent.material.SetTexture("_MainTex", imgComponent.sprite.ExtractSpriteTexture());
    }
}
