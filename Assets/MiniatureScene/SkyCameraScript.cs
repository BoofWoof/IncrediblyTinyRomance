using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCameraScript : MonoBehaviour
{
    public Camera skyboxCamera;
    public RenderTexture cubemap;

    void Update()
    {
        // Capture the scene as a cubemap
        skyboxCamera.RenderToCubemap(cubemap);
        RenderSettings.skybox.SetTexture("_Tex", cubemap);
    }
}
