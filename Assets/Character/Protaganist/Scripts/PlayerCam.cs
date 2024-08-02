using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private Vector2 CameraInput = Vector2.zero;
    private PlayerControls inputs;

    private void Awake()
    {
        inputs = new PlayerControls();
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        inputs.Overworld.Camera.performed += context => CameraInput = context.ReadValue<Vector2>();
        inputs.Overworld.Camera.canceled += context => CameraInput = Vector2.zero;
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
    }

    public void Update()
    {
        float mouseX = CameraInput.x * Time.deltaTime * sensX;
        float mouseY = CameraInput.y * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
