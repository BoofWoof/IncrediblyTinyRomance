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
        float normalizedAnger = PrayerScript.instance.GetAngerLevel();

        if (PrayerScript.instance.JudgementActive)
        {
            SetAnger(normalizedAnger * PrayerScript.instance.AngerThreshold);
        }

        base.UpdateAnger();

        Color angerColor = EmotionColors.Evaluate(Anger / PrayerScript.instance.AngerThreshold);

        PointLight1.color = angerColor;
        PointLight2.color = angerColor;
        if(TubeMaterial != null) TubeMaterial.SetColor("_EmissionColor", angerColor * 5f);
        if (ArmMaterial != null) ArmMaterial.SetColor("_EmissionColor", angerColor * 5f);

        foreach (SkinnedMeshRenderer emotiveMesh in EmotiveMeshes)
        {
            int blendshapeIndex = emotiveMesh.sharedMesh.GetBlendShapeIndex("Angry");
            emotiveMesh.SetBlendShapeWeight(blendshapeIndex, Anger*100f/PrayerScript.instance.AngerThreshold);
        }
    }
}
