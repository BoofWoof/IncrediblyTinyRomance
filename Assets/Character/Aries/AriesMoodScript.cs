using UnityEngine;

public class AriesMoodScript : MoodInterface
{
    [Header ("Aries Components")]
    public SkinnedMeshRenderer HeadMesh;
    public Material TubeMaterial;
    public Light PointLight1;
    public Light PointLight2;

    public Gradient EmotionColors;

    public override void UpdateAnger()
    {
        base.UpdateAnger();

        float normalizedAnger = Mathf.InverseLerp(MinAnger, MaxAnger, Anger);

        Color angerColor = EmotionColors.Evaluate(normalizedAnger);

        PointLight1.color = angerColor;
        PointLight2.color = angerColor;
        TubeMaterial.SetColor("_EmissionColor", angerColor * 2f);

        int blendshapeIndex = HeadMesh.sharedMesh.GetBlendShapeIndex("Angry");
        HeadMesh.SetBlendShapeWeight(blendshapeIndex, normalizedAnger * 100f);
    }
}
