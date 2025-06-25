using UnityEngine;

public class SetInitialPoster : MonoBehaviour
{
    public int InitialPosterIdx = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", UnlockablesManager.PostersList[InitialPosterIdx].Image.texture);
    }
}
