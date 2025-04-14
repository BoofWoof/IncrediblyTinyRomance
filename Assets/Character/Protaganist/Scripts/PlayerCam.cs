using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    public float yRotation;

    private Vector2 CameraInput = Vector2.zero;
    private PlayerControls inputs;

    public AudioSource PhoneShiftAudioSource;
    public AudioClip RaisePhoneClip;
    public AudioClip LowerPhoneClip;

    public ScreenMaskScript ScreenMask;

    [Header("Activation")]
    public float MaxActivationDistance = 2f;

    private Button currentButton;

    public static bool EnableCameraMovement = false;

    private void Awake()
    {
        inputs = new PlayerControls();
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        inputs.Overworld.Camera.performed += context => CameraInput = context.ReadValue<Vector2>();
        inputs.Overworld.Camera.canceled += context => CameraInput = Vector2.zero;
        inputs.Overworld.ActivateObject.performed += context => ActivateObjects();
    }

    private void ActivateObjects()
    {
        if (PhonePositionScript.raised)
        {
            return;
        }
        // Create a ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, MaxActivationDistance))
        {
            // Call a function on the object (like an Activate method)
            ActivatableObjectScript aos = hit.collider.gameObject.GetComponent<ActivatableObjectScript>();
            if (aos != null)
            {
                aos.Activate();
            }
        }
    }
    private void OnEnable()
    {
        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PhonePositionScript.PhoneToggled += PhoneToggle;
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

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            inputs.Disable();
        }
        else
        {
            PhoneShiftAudioSource.clip = LowerPhoneClip;
            PhoneShiftAudioSource.Play();
            ScreenMask.ShutDownScreen();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inputs.Enable();
        }
    }

    public void Update()
    {
        if (!EnableCameraMovement) return;
        float mouseX = CameraInput.x * Time.deltaTime * sensX;
        float mouseY = CameraInput.y * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector3 newOrientation = new Vector3 (xRotation, yRotation, 0) + MoveCamera.rumble * Random.insideUnitSphere * 15f + MoveCamera.shake * Random.insideUnitSphere * 15f;
        transform.rotation = Quaternion.Euler(newOrientation);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
