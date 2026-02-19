using System.Collections.Generic;
using UnityEngine;

public class AriesMoodScript : MoodInterface
{
    [Header ("Aries Components")]
    public List<SkinnedMeshRenderer> EmotiveMeshes;
    public Material TubeMaterial;
    public Material ArmMaterial;
    public Light PointLight1;
    public Light PointLight2;

    public Gradient EmotionColors;

    public override void UpdateAnger()
    {
        if (PrayerScript.instance.JudgementActive)
        {
            SetAnger(PrayerScript.instance.GetAngerLevel() * 100f);
        }

        base.UpdateAnger();

        float normalizedAnger = Mathf.InverseLerp(MinAnger, MaxAnger, Anger);

        Color angerColor = EmotionColors.Evaluate(normalizedAnger);

        PointLight1.color = angerColor;
        PointLight2.color = angerColor;
        TubeMaterial.SetColor("_EmissionColor", angerColor * 5f);
        ArmMaterial.SetColor("_EmissionColor", angerColor * 5f);

        foreach (SkinnedMeshRenderer emotiveMesh in EmotiveMeshes)
        {
            int blendshapeIndex = emotiveMesh.sharedMesh.GetBlendShapeIndex("Angry");
            emotiveMesh.SetBlendShapeWeight(blendshapeIndex, normalizedAnger * 100f);
        }
    }
}
