using UnityEngine;

public class ClutterSpinnerScript : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;
    public bool invertY = true; // Optional: invert vertical movement

    private bool dragging = false;
    private Vector3 lastMousePos;

    void Update()
    {
        // Start dragging on left mouse down
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            lastMousePos = Input.mousePosition;
        }

        // Stop dragging on mouse up
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        // Apply rotation while dragging
        if (dragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            float rotX = delta.y * rotationSpeed * (invertY ? 1 : -1);
            float rotY = -delta.x * rotationSpeed;

            transform.Rotate(rotX, rotY, 0, Space.World);

            lastMousePos = Input.mousePosition;
        }
    }
}
