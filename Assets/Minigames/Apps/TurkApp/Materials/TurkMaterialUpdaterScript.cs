using UnityEngine;

public class TurkMaterialUpdaterScript : MonoBehaviour
{
    public Material emptyMaterial;

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        float newX = mousePosition.x;
        float newY = Screen.height - mousePosition.y;
        Vector2 newMousePosition = new Vector2(newX, newY);
        Shader.SetGlobalVector("_MousePosition", newMousePosition);
        Shader.SetGlobalFloat("_RevealStrength", 2f);
    }
}
