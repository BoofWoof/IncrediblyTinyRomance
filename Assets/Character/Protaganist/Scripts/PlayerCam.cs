using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    public AudioSource PhoneShiftAudioSource;
    public AudioClip RaisePhoneClip;
    public AudioClip LowerPhoneClip;

    public ScreenMaskScript ScreenMask;

    [Header("Activation")]
    public float MaxActivationDistance = 2f;

    public static bool EnableCameraMovement = true;

    public static PlayerCam Instance;

    private void Awake()
    {
        RegisterInputActions();
    }
    private void Start()
    {
        Instance = this;
        EnableCameraMovement = false;
        PhonePositionScript.PhoneToggled += PhoneToggle;
    }
    private void RegisterInputActions()
    {
        InputManager.PlayerInputs.Overworld.Camera.performed += context => CameraInput = context.ReadValue<Vector2>();
        InputManager.PlayerInputs.Overworld.Camera.canceled += context => CameraInput = Vector2.zero;
        InputManager.PlayerInputs.Overworld.ActivateObject.performed += context => ActivateObjects();
    }

    private void ActivateObjects()
    {
        if (PhonePositionScript.raised || CursorStateControl.MenuUp || Cursor.lockState == CursorLockMode.Confined)
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

    public void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Time.timeScale = 1.0f;
            AudioListener.pause = false; // Resume audio when gaining focus
        }
        else
        {
            Time.timeScale = 0f;
            AudioListener.pause = true; // Mute audio when losing focus
        }
    }

    public void Update()
    {
        if (!EnableCameraMovement || CursorStateControl.MenuUp || Cursor.lockState == CursorLockMode.Confined) return;
        float mouseX = CameraInput.x * Time.deltaTime * sensX;
        float mouseY = CameraInput.y * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector3 newOrientation = new Vector3 (xRotation, yRotation, 0) + MoveCamera.TotalRumble * Random.insideUnitSphere * 15f;
        transform.rotation = Quaternion.Euler(newOrientation);
        orientation.rotation = Quaternion.Euler(new Vector3(0, yRotation, 0));
    }
}
