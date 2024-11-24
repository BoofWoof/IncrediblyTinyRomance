using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBounce : MonoBehaviour
{
    private Vector3 StartingPos;
    public float Amplitude;
    public float Frequency;
    public float Phase;

    public void Start()
    {
        StartingPos = transform.position;
    }

    public void Update()
    {
        transform.position = StartingPos + transform.up * Amplitude * Mathf.Sin(Frequency*Time.time*Mathf.PI*2f + Phase);
    }
}
