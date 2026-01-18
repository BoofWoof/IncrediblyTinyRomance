using PixelCrushers.DialogueSystem.Articy.Articy_4_0;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum BADdirections
{
    UP, DOWN, LEFT, RIGHT, NULL
}
public static class BADDirectionExtensions
{
    public static BADdirections Flipped(this BADdirections dir)
    {
        switch (dir)
        {
            case BADdirections.UP: return BADdirections.DOWN;
            case BADdirections.DOWN: return BADdirections.UP;
            case BADdirections.LEFT: return BADdirections.RIGHT;
            case BADdirections.RIGHT: return BADdirections.LEFT;
            case BADdirections.NULL: return BADdirections.NULL;
            default: throw new System.ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}

public class PipeStackScript : MonoBehaviour
{
    public VentGridScript GridSource;
    public PipeStruct PipeStructData;
    [HideInInspector] public int VentPosID;

    public PipeSOHolder PipeTypes;
    [HideInInspector] public int PipeTypeIdx = 0;

    [Header ("Components")]
    public GameObject Pipe;
    public GameObject PipeSecondLayer;
    public GameObject Backdrop;
    public GameObject LightLayer;
    public GameObject FanLayer;
    public GameObject Gear;

    [Header("Praticle Components")]
    public GameObject SourceParticles;
    public GameObject SinkParticles;
    public GameObject UpLeakParticles;
    public GameObject RightLeakParticles;
    public GameObject DownLeakParticles;
    public GameObject LeftLeakParticles;

    public bool canRotate = true;
    public bool fanSpinning = false;

    public bool isNormalWithCaps = false;
    public bool isGoal = false;
    public bool isSource = false;
    public bool isCapped = false;

    public bool lightActive = false;

    //[Header ("Connections With Rotation")]
    public PipeConnectionType UpConnection;
    public bool UpSecondary;
    public PipeConnectionType DownConnection;
    public bool DownSecondary;
    public PipeConnectionType LeftConnection;
    public bool LeftSecondary;
    public PipeConnectionType RightConnection;
    public bool RightSecondary;

    [Header("Rotation Parameters")]
    [HideInInspector] public int RotationTracker = 0;

    public float RotationPeriod = 0.5f;

    [HideInInspector] public bool Rotating;

    [Header("Lights")]
    public Sprite IdleLight;
    public Sprite DisconnectedLight;
    public Sprite InUseLight;

    public delegate void VentRotation();
    public static VentRotation VentRotationEvent;
    public static VentRotation VentRotationStartEvent;

    static private PipeConnectionType[] RotationLoop = new PipeConnectionType[4] { 
        PipeConnectionType.UpConnected,
        PipeConnectionType.RightConnected,
        PipeConnectionType.DownConnected,
        PipeConnectionType.LeftConnected
    };

    public static bool GlobalRotationAllowed = false;

    public void SetSource()
    {
        if (PipeStructData.SourceVersion == null) return;

        isCapped = true;
        isGoal = false;
        isSource = true;
        isNormalWithCaps = false;

        Pipe.GetComponent<Image>().sprite = PipeStructData.SourceVersion;
        PipeSecondLayer.SetActive(false);

        FanLayer.SetActive(true);

        GridSource.Sources.Remove(this);
        GridSource.Goals.Remove(this);
        GridSource.Sources.Add(this);
        GridSource.Goals.Add(this);

        SourceParticles.SetActive(true);
        SinkParticles.SetActive(false);

        ConnectionRotationUpdate();
        CapOverride();
    }

    public void SetSink()
    {
        if (PipeStructData.SinkVersion == null) return;

        isCapped = true;
        isGoal = true;
        isSource = false;
        isNormalWithCaps = false;

        Pipe.GetComponent<Image>().sprite = PipeStructData.SinkVersion;
        PipeSecondLayer.SetActive(false);

        FanLayer.SetActive(true);

        GridSource.Sources.Remove(this);
        GridSource.Goals.Remove(this);
        GridSource.Goals.Add(this);

        SourceParticles.SetActive(false);
        SinkParticles.SetActive(false);

        ConnectionRotationUpdate();
        CapOverride();
    }

    public void SetCap()
    {
        if (PipeStructData.CapVersion == null) return;

        isCapped = true;
        isGoal = false;
        isSource = false;
        isNormalWithCaps = false;

        Pipe.GetComponent<Image>().sprite = PipeStructData.CapVersion;
        PipeSecondLayer.SetActive(false);

        FanLayer.SetActive(false);

        GridSource.Sources.Remove(this);
        GridSource.Goals.Remove(this);

        SourceParticles.SetActive(false);
        SinkParticles.SetActive(false);

        ConnectionRotationUpdate();
        CapOverride();
    }

    public void SetNormal()
    {
        isCapped = PipeStructData.ForceCapped;
        isGoal = false;
        isSource = false;
        isNormalWithCaps = false;

        if (PipeStructData.ShowFan)
        {
            FanLayer.SetActive(true);
        } else
        {
            FanLayer.SetActive(false);
        }

        if (PipeStructData.PipeSprite)
        {
            Pipe.SetActive(true);
            Pipe.GetComponent<Image>().sprite = PipeStructData.PipeSprite;
        }
        else
        {
            Pipe.SetActive(false);
        }

        Image secondaryImage = PipeSecondLayer.GetComponent<Image>();
        if(PipeStructData.SecondaryPipeSprite == null)
        {
            PipeSecondLayer.SetActive(false);
        } else
        {
            PipeSecondLayer.SetActive(true);
            secondaryImage.sprite = PipeStructData.SecondaryPipeSprite;
        }

        GridSource.Sources.Remove(this);
        GridSource.Goals.Remove(this);

        SourceParticles.SetActive(false);
        SinkParticles.SetActive(false);

        ConnectionRotationUpdate();
        CapOverride();
    }

    public void SetNormalWithCaps()
    {
        if (PipeTypeIdx <= 3 || PipeTypeIdx == 7) return;

        isNormalWithCaps = true;
        isCapped = false;
        isGoal = false;
        isSource = false;

        if (PipeStructData.ShowFan)
        {
            FanLayer.SetActive(true);
        }
        else
        {
            FanLayer.SetActive(false);
        }

        Pipe.SetActive(true);
        Pipe.GetComponent<Image>().sprite = PipeStructData.PipeSprite;

        Image secondaryImage = PipeSecondLayer.GetComponent<Image>();
        PipeSecondLayer.SetActive(true);
        secondaryImage.sprite = PipeStructData.SecondaryCapSprite;

        GridSource.Sources.Remove(this);
        GridSource.Goals.Remove(this);

        SourceParticles.SetActive(false);
        SinkParticles.SetActive(false);

        ConnectionRotationUpdate();
        CloseOpenings();
    }

    public void CloseOpenings()
    {
        if (!isNormalWithCaps) return;
        if (UpConnection == PipeConnectionType.Closed) UpConnection = PipeConnectionType.Capped;

        if (RightConnection == PipeConnectionType.Closed) RightConnection = PipeConnectionType.Capped;

        if (DownConnection == PipeConnectionType.Closed) DownConnection = PipeConnectionType.Capped;

        if (LeftConnection == PipeConnectionType.Closed) LeftConnection = PipeConnectionType.Capped;
    }

    public void CapOverride()
    {
        if (!isCapped) return;
        if (UpConnection != PipeConnectionType.Closed) UpConnection = PipeConnectionType.Capped;
        UpSecondary = false;

        if (RightConnection != PipeConnectionType.Closed) RightConnection = PipeConnectionType.Capped;
        RightSecondary = false;

        if (DownConnection != PipeConnectionType.Closed) DownConnection = PipeConnectionType.Capped;
        DownSecondary = false;

        if (LeftConnection != PipeConnectionType.Closed) LeftConnection = PipeConnectionType.Capped;
        LeftSecondary = false;
    }

    public void EnableRotation()
    {
        canRotate = true;
        Gear.SetActive(true);
    }

    public void DisableRotation()
    {
        canRotate = false;
        Gear.SetActive(false);
    }

    public void ConnectionRotationUpdate()
    {
        UpConnection = RotateConnection(0);
        UpSecondary = RotateSecondary(0);

        RightConnection = RotateConnection(1);
        RightSecondary = RotateSecondary(1);

        DownConnection = RotateConnection(2);
        DownSecondary = RotateSecondary(2);

        LeftConnection = RotateConnection(3);
        LeftSecondary = RotateSecondary(3);
    }

    public void SetLightIdle()
    {
        LightLayer.GetComponent<Image>().sprite = IdleLight;
        fanSpinning = false;
        lightActive = false;
    }

    public void SetLightActive()
    {
        LightLayer.GetComponent<Image>().sprite = InUseLight;
        fanSpinning = true;
        lightActive = true;
    }

    public void SetLightLeaking()
    {
        LightLayer.GetComponent<Image>().sprite = DisconnectedLight;
        fanSpinning = false;
        lightActive = false;
    }

    public void Update()
    {
        if (isGoal) SinkParticles.SetActive(lightActive);
        if (isSource || fanSpinning) FanLayer.transform.Rotate(0, 0, - Time.deltaTime * 1000f);

        int mask = ~(1 << LayerMask.NameToLayer("ZoomRaycast"));
        if (Input.GetMouseButtonDown(0)) // Right click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5, mask))
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

            if (Physics.Raycast(ray, out hit, 5, mask))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Right-clicked on: " + gameObject.name);
                    RotateCW();
                }
            }
        }
    }

    public void ResetParticleSystems()
    {
        LeftLeakParticles.SetActive(false);
        RightLeakParticles.SetActive(false);
        UpLeakParticles.SetActive(false);
        DownLeakParticles.SetActive(false);
    }

    public void Start()
    {
        InstantRotation();
        //SetPipeType(PipeTypeIdx);
    }

    public void RotateCW()
    {
        if (!GlobalRotationAllowed) return;
        if (Rotating || !canRotate) return;
        StartCoroutine(RotatePipe(1));
    }

    public void RotateCC()
    {
        if (!GlobalRotationAllowed) return;
        if (Rotating || !canRotate) return;
        StartCoroutine(RotatePipe(-1));
    }

    public IEnumerator RotatePipe(int rotationChange)
    {
        VentRotationStartEvent?.Invoke();

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
            if (isGoal) newRotation += 45;
            Gear.transform.rotation = Quaternion.Euler(0f, 0f, -newRotation);
            yield return null;
        }

        Pipe.transform.rotation = Quaternion.Euler(0f, 0f, -finalRotation);
        if (isGoal) finalRotation += 45;
        Gear.transform.rotation = Quaternion.Euler(0f, 0f, -finalRotation);

        SetRotationTracker(RotationTracker + rotationChange);

        Rotating = false;

        ConnectionRotationUpdate();
        CapOverride();
        CloseOpenings();

        VentRotationEvent?.Invoke();
    }

    public void SetPipeType(int PipeIdx)
    {
        if (PipeTypes.PipeStructs.Count <= PipeIdx || PipeIdx < 0) return;

        PipeTypeIdx = PipeIdx;
        PipeStruct pipeData = PipeTypes.PipeStructs[PipeIdx];

        if(PipeTypeIdx == 0) canRotate = false;

        PipeStructData = pipeData;

        if (isSource) SetSource();
        else if (isGoal) SetSink();
        else if (isCapped) SetCap();
        else if (isNormalWithCaps) SetNormalWithCaps();
        else SetNormal();

        if (!pipeData.EnableBackdrop) HideBackdrop();
        else ShowBackdrop();

        if (canRotate) Gear.SetActive(true);
        else Gear.SetActive(false);

        InstantRotation();

        ResetParticleSystems();
    }

    public bool RotateSecondary(int connectionIdx)
    {
        //Up = 0
        //Right = 1
        //Down = 2
        //Left = 3
        int updatedIdx = (connectionIdx - RotationTracker) % 4;
        if (updatedIdx < 0) updatedIdx += 4;

        switch (updatedIdx)
        {
            case 0:
                return PipeStructData.UpSecondaryVentConnection;
            case 1:
                return PipeStructData.RightSecondaryVentConnection;
            case 2:
                return PipeStructData.DownSecondaryVentConnection;
            case 3:
                return PipeStructData.LeftSecondaryVentConnection;
        }
        return false;
    }

    public PipeConnectionType RotateConnection(int connectionIdx)
    {
        //Up = 0
        //Right = 1
        //Down = 2
        //Left = 3
        int updatedIdx = (connectionIdx - RotationTracker)%4;
        if (updatedIdx < 0) updatedIdx += 4;

        PipeConnectionType connectionType = PipeConnectionType.Closed;

        switch (updatedIdx)
        {
            case 0:
                connectionType = PipeStructData.UpConnection;
                break;
            case 1:
                connectionType = PipeStructData.RightConnection;
                break;
            case 2:
                connectionType = PipeStructData.DownConnection;
                break;
            case 3:
                connectionType = PipeStructData.LeftConnection;
                break;
        }

        if (connectionType == PipeConnectionType.Closed || connectionType == PipeConnectionType.All) return connectionType;

        int connectionTypeIdx = Array.IndexOf(RotationLoop, connectionType);
        connectionTypeIdx = (connectionTypeIdx + RotationTracker) % 4;
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
        float rotation = -RotationTracker * 90f;
        Pipe.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        if (isGoal) rotation += 45f;
        Gear.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
    }
    public void HideBackdrop()
    {
        if (Backdrop) Backdrop.GetComponent<Image>().color = Color.grey;
        if (LightLayer) LightLayer.SetActive(false);
    }
    public void ShowBackdrop()
    {
        if (Backdrop) Backdrop.GetComponent<Image>().color = Color.white;
        if (LightLayer) LightLayer.SetActive(true);
    }

    public List<BADdirections> GetPossibleExpansions(BADdirections fumeEntranceVelocity)
    {
        if (fumeEntranceVelocity == BADdirections.NULL) return GetNullExpansions();

        List<BADdirections> expansionList = new List<BADdirections>();

        PipeConnectionType sourceConnectionType = PipeConnectionType.Closed;

        //If the fumes are heading this direction, what pipe do they enter, and where is that pipe going to?
        switch (fumeEntranceVelocity)
        {
            case BADdirections.LEFT:
                sourceConnectionType = RightConnection;
                break;
            case BADdirections.RIGHT:
                sourceConnectionType = LeftConnection;
                break;
            case BADdirections.UP:
                sourceConnectionType = DownConnection;
                break;
            case BADdirections.DOWN:
                sourceConnectionType = UpConnection;
                break;
        }

        //Based on where the fumes are going, what pipes will it exit from.
        switch (sourceConnectionType){
            case PipeConnectionType.UpConnected:
                expansionList.Add(BADdirections.UP);
                break;
            case PipeConnectionType.RightConnected:
                expansionList.Add(BADdirections.RIGHT);
                break;
            case PipeConnectionType.DownConnected:
                expansionList.Add(BADdirections.DOWN);
                break;
            case PipeConnectionType.LeftConnected:
                expansionList.Add(BADdirections.LEFT);
                break;
            case PipeConnectionType.All:
                expansionList = GetNullExpansions();
                expansionList.Remove(fumeEntranceVelocity.Flipped());
                break;
        }

        return expansionList;
    }

    private List<BADdirections> GetNullExpansions()
    {
        List<BADdirections> expansionList = new List<BADdirections>();

        if (UpConnection != PipeConnectionType.Closed) expansionList.Add(BADdirections.UP);
        if (DownConnection != PipeConnectionType.Closed) expansionList.Add(BADdirections.DOWN);
        if (LeftConnection != PipeConnectionType.Closed) expansionList.Add(BADdirections.LEFT);
        if (RightConnection != PipeConnectionType.Closed) expansionList.Add(BADdirections.RIGHT);

        return expansionList;
    }
}
