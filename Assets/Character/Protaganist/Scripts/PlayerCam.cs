using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    public float yRotation;

    private Vector2 CameraInput = Vector2.zero;

    public AudioSource PhoneShiftAudioSource;
    public AudioClip RaisePhoneClip;
    public AudioClip LowerPhoneClip;

    public ScreenMaskScript ScreenMask;

    [Header("Activation")]
    public float MaxActivationDistance = 2f;
    private GameObject TargetActivationObject = null;

    public static bool EnableCameraMovement = true;

    public static PlayerCam Instance;

    private static float MouseSensitivityMultiplier = 1f;
    private float SlowModeMultiplier = 1f;


    private void Awake()
    {
        RegisterInputActions();
    }
    private void Start()
    {
        Instance = this;
        EnableCameraMovement = false;
        PhonePositionScript.PhoneToggled += PhoneToggle;
        PhonePositionScript.PhoneToggled += ClearScreenPointSelection;

    }
    private void RegisterInputActions()
    {
        InputManager.PlayerInputs.Overworld.Camera.performed += context => CameraInput = context.ReadValue<Vector2>();
        InputManager.PlayerInputs.Overworld.Camera.canceled += context => CameraInput = Vector2.zero;
        InputManager.PlayerInputs.Overworld.ActivateObject.performed += context => ActivateObjects();
    }

    public static void SetMouseSensitivity(float newMouseSensitivity)
    {
        MouseSensitivityMultiplier = newMouseSensitivity;
    }

    private void ActivateObjects()
    {
        if (!EnableCameraMovement || CursorStateControl.MenuUp || Cursor.lockState == CursorLockMode.Confined) return;
        if (TargetActivationObject == null) return;
        ActivatableObjectScript aos = TargetActivationObject.GetComponent<ActivatableObjectScript>();
        if (aos != null)
        {
            aos.Activate();
        }
    }
    private void OnDestroy()
    {
        PhonePositionScript.PhoneToggled -= PhoneToggle;
    }

    private void PhoneToggle(bool raised)
    {
        if (raised)
        {
            PhoneShiftAudioSource.clip = RaisePhoneClip;
            PhoneShiftAudioSource.Play();
            ScreenMask.StartScreen();
        }
        else
        {
            PhoneShiftAudioSource.clip = LowerPhoneClip;
            PhoneShiftAudioSource.Play();
            ScreenMask.ShutDownScreen();
        }
    }
    public void SetSpeed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SlowModeMultiplier = 0.3f;
        }
        if (context.canceled)
        {
            SlowModeMultiplier = 1f;
        }
    }

    public void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            ActiveBroadcast.BroadcastActivation("ForceGamePause");
        }
    }

    public void ClearScreenPointSelection(bool PhoneUp)
    {
        if (!PhoneUp) return;

        if (TargetActivationObject != null)
        {
            TargetActivationObject.layer = LayerMask.NameToLayer("Default");
            TargetActivationObject = null;
            ReticleScript.instance.SetDefault();
        }
    }

    public void Update()
    {
        if (!EnableCameraMovement || CursorStateControl.MenuUp || Cursor.lockState == CursorLockMode.Confined) return;
        float mouseX = CameraInput.x * Time.deltaTime * sensX * SlowModeMultiplier * MouseSensitivityMultiplier;
        float mouseY = CameraInput.y * Time.deltaTime * sensY * SlowModeMultiplier * MouseSensitivityMultiplier;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector3 newOrientation = new Vector3 (xRotation, yRotation, 0) + MoveCamera.TotalRumble * Random.insideUnitSphere * 15f;
        transform.rotation = Quaternion.Euler(newOrientation);
        orientation.rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));


        // Create a ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, MaxActivationDistance))
        {
            ActivatableObjectScript aos = hit.collider.gameObject.GetComponent<ActivatableObjectScript>();
            if (aos != null)
            {
                if (TargetActivationObject != null)
                {
                    TargetActivationObject.layer = LayerMask.NameToLayer("Default");
                    ReticleScript.instance.SetDefault();
                }
                TargetActivationObject = hit.collider.gameObject;
                if (aos.ObjectEnabled)
                {
                    TargetActivationObject.layer = LayerMask.NameToLayer("Outline");
                    ReticleScript.instance.SetInspector();
                }
            } else
            {
                if (TargetActivationObject != null)
                {
                    TargetActivationObject.layer = LayerMask.NameToLayer("Default");
                    TargetActivationObject = null;
                    ReticleScript.instance.SetDefault();
                }
            }
        } else
        {
            if(TargetActivationObject != null)
            {
                TargetActivationObject.layer = LayerMask.NameToLayer("Default");
                TargetActivationObject = null;
                ReticleScript.instance.SetDefault();
            }
        }
        if(!Physics.Raycast(ray, 20))
        {
            BigCameraPoint.instance.ScanForTarget();
        } else
        {
            BigCameraPoint.instance.Clear();
        }
    }
}
