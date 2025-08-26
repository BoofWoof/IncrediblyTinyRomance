using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ADTargetScript : MonoBehaviour
{
    public RectTransform canvasRect;   // Your world-space canvas
    public Camera worldCamera;         // The camera looking at the canvas
    public RectTransform thisRect;

    public static bool ValidTarget = false;

    public void Start()
    {
        thisRect = GetComponent<RectTransform>();
        worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (AerialDefenseScript.Locked) return;

        ValidTarget = false;

        // Get mouse position in screen space
        Vector2 mousePos = Input.mousePosition;

        // Convert mouse position to local position inside canvas
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePos, worldCamera, out localPoint))
        {
            if (localPoint.x > 950) return;
            if (localPoint.x < -950) return;
            if (localPoint.y > 500) return;
            if (localPoint.y < -300) return;
            thisRect.anchoredPosition = localPoint;
            ValidTarget = true;
        }

    }
}
