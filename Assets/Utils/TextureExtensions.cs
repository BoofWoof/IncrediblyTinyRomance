using UnityEngine;

public static class TextureExtensions
{
    public static Texture2D ExtractSpriteTexture(this Sprite sprite)
    {
        Texture2D atlas = sprite.texture;
        Rect rect = sprite.rect;

        // Create a new texture with the size of the sprite
        Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height);
        newTex.SetPixels(atlas.GetPixels(
            (int)rect.x, (int)rect.y,
            (int)rect.width, (int)rect.height));
        newTex.Apply();

        return newTex;
    }
}
