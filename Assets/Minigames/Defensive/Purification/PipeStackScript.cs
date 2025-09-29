using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PipeStackScript : MonoBehaviour
{
    public PipeSOHolder PipeTypes;
    public int PipeTypeIdx = 0;

    public GameObject Pipe;

    [Header("Connections Without Rotation")]
    public PipeConnectionType BaseUpConnection;
    public PipeConnectionType BaseDownConnection;
    public PipeConnectionType BaseLeftConnection;
    public PipeConnectionType BaseRightConnection;

    [Header ("Connections With Rotation")]
    public PipeConnectionType UpConnection;
    public PipeConnectionType DownConnection;
    public PipeConnectionType LeftConnection;
    public PipeConnectionType RightConnection;

    [Header("Rotation Parameters")]
    public int RotationTracker = 0;

    public float RotationPeriod = 0.5f;

    public bool Rotating;

    static private PipeConnectionType[] RotationLoop = new PipeConnectionType[4] { 
        PipeConnectionType.UpConnected,
        PipeConnectionType.RightConnected,
        PipeConnectionType.DownConnected,
        PipeConnectionType.LeftConnected
    };

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Right click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Right-clicked on: " + gameObject.name);
                    RotateCC();
                }
            }
        }
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Right-clicked on: " + gameObject.name);
                    RotateCW();
                }
            }
        }
    }

    public void Start()
    {
        InstantRotation();
        SetPipeType(PipeTypeIdx);
    }

    public void RotateCW()
    {
        if (Rotating) return;
        StartCoroutine(RotatePipe(1));
    }

    public void RotateCC()
    {
        if (Rotating) return;
        StartCoroutine(RotatePipe(-1));
    }

    public IEnumerator RotatePipe(int rotationChange)
    {
        Rotating = true;
        float currentRotations = RotationTracker * 90f;
        float finalRotation = (RotationTracker + rotationChange) * 90f;

        float time = 0f;
        while (time < RotationPeriod)
        {
            time += Time.deltaTime;
            float progress = time / RotationPeriod;

            float newRotation = Mathf.Lerp(currentRotations, finalRotation, progress);
            Pipe.transform.rotation = Quaternion.Euler(0f, 0f, -newRotation);
            yield return null;
        }

        Pipe.transform.rotation = Quaternion.Euler(0f, 0f, -finalRotation);

        SetRotationTracker(RotationTracker + rotationChange);

        Rotating = false;
    }

    public void SetPipeType(int PipeIdx)
    {
        if (PipeTypes.PipeStructs.Count <= PipeIdx || PipeIdx < 0) return;
        PipeTypeIdx = PipeIdx;
        PipeStruct pipeData = PipeTypes.PipeStructs[PipeIdx];

        BaseUpConnection = pipeData.UpConnection;
        BaseRightConnection = pipeData.RightConnection;
        BaseDownConnection = pipeData.DownConnection;
        BaseLeftConnection = pipeData.LeftConnection;

        ConnectionRotationUpdate();

        Pipe.GetComponent<Image>().sprite = pipeData.PipeSprite;
    }

    public void ConnectionRotationUpdate()
    {
        UpConnection = RotateConnection(0);
        RightConnection = RotateConnection(1);
        DownConnection = RotateConnection(2);
        LeftConnection = RotateConnection(3);
    }

    public PipeConnectionType RotateConnection(int connectionIdx)
    {
        //Up = 0
        //Right = 1
        //Down = 2
        //Left = 3
        int updatedIdx = (connectionIdx + RotationTracker)%4;
        if (updatedIdx < 0) updatedIdx += 4;

        PipeConnectionType connectionType = PipeConnectionType.Closed;

        switch (updatedIdx)
        {
            case 0:
                connectionType = BaseUpConnection;
                break;
            case 1:
                connectionType = BaseRightConnection;
                break;
            case 2:
                connectionType = BaseDownConnection;
                break;
            case 3:
                connectionType = BaseLeftConnection;
                break;
        }

        if (connectionType == PipeConnectionType.Closed || connectionType == PipeConnectionType.All) return connectionType;

        int connectionTypeIdx = Array.IndexOf(RotationLoop, connectionType);
        connectionTypeIdx = (connectionTypeIdx - RotationTracker) % 4;
        if (connectionTypeIdx < 0) connectionTypeIdx += 4;
        return RotationLoop[connectionTypeIdx];
    }

    public void SetRotationTracker(int newRotation)
    {
        RotationTracker = newRotation%4;
        if(RotationTracker < 0) RotationTracker += 4;
        ConnectionRotationUpdate();
    }

    public void InstantRotation()
    {
        Pipe.transform.rotation = Quaternion.Euler(0f, 0f, -RotationTracker * 90f);
    }
}
