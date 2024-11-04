using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenHealthScript : MonoBehaviour
{
    public RectMask2D BGMask1;
    public RectMask2D BGMask2;
    public GameObject BGMaskParticles;
    public float HealthBalance = 0;
    public float ParticleWidth = 0.53f;

    void Update()
    {
        BGMask1.padding = new Vector4(0, 0, 800 - HealthBalance*10, 0);
        BGMask2.padding = new Vector4(0, 0, 800 - HealthBalance*10, 0);
        BGMaskParticles.transform.localPosition = new Vector3(ParticleWidth*HealthBalance/800f, 0, 0.891f);
    }
}
