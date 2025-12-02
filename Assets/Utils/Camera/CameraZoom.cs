using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    public Camera TargetCamera;

    public float DefaultFOV;
    public float ZoomFOV;
    private float TargetFOV;
    private float CurrentFOV;

    public float FOVSpeed;

    private bool ZoomEnabled;

    public void Awake()
    {
        TargetFOV = DefaultFOV;
        CurrentFOV = DefaultFOV;
        ZoomEnabled = true;
    }

    public void Update()
    {
        if (TargetCamera == null) return;
        CurrentFOV = Mathf.MoveTowards(CurrentFOV, TargetFOV, FOVSpeed * Time.deltaTime);

        TargetCamera.fieldOfView = CurrentFOV;
    }

    public void ZoomChange(InputAction.CallbackContext context)
    {
        if (!ZoomEnabled) return;

        if (context.started)
        {
            Debug.Log("ZOOM");
            TargetFOV = ZoomFOV;
        }
        if (context.canceled)
        {
            Debug.Log("UnZOOM");
            TargetFOV = DefaultFOV;
        }

    }

    public void OnEnable()
    {
        PhonePositionScript.PhoneToggled += EnableZoom;
    }

    public void EnableZoom(bool phoneUp)
    {
        ZoomEnabled = !phoneUp;
        if(phoneUp)
        {
            TargetFOV = DefaultFOV;
        }
    }
}
