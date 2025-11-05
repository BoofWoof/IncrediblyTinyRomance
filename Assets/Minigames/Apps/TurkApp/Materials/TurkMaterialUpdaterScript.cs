using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TurkMaterialUpdaterScript : MonoBehaviour
{
    public Material emptyMaterial;

    public delegate void MaterialValueModifier(ref float originalStrength);
    public static MaterialValueModifier VisionStrengthModifier;

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        float newX = mousePosition.x;
        float newY = Screen.height - mousePosition.y;
        Vector2 newMousePosition = new Vector2(newX, newY);
        Shader.SetGlobalVector("_MousePosition", newMousePosition);

        float visionStrength = TurkData.VisionStrength;
        VisionStrengthModifier?.Invoke(ref visionStrength);
        if(visionStrength < TurkData.VisionStrength) visionStrength = TurkData.VisionStrength;
        Shader.SetGlobalFloat("_RevealStrength", visionStrength);
    }
}
