using NUnit.Framework;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ADTargetScript : MonoBehaviour
{
    public RectTransform canvasRect;   // Your world-space canvas
    public Camera worldCamera;         // The camera looking at the canvas
    public RectTransform thisRect;

    public int targetIdx = 0;

    public static bool ValidTarget = false;

    public void Start()
    {
        thisRect = GetComponent<RectTransform>();
        worldCamera = Camera.main;

        TurretScript.TurretFiredEvent += TurretFireEvent;
    }

    // Update is called once per frame
    void Update()
    {
        if (AerialDefenseScript.GameRunning)
        {
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
        } else
        {
        }

    }

    public void TurretFireEvent(TurretScript turretScript)
    {
        if (!AerialDefenseScript.GameRunning)
        {
            if (FallingThreatScript.FallingThreatScripts.Count > 0)
            {
                targetIdx++;
                if (FallingThreatScript.FallingThreatScripts.Count >= targetIdx)
                {
                    targetIdx = 0;
                }
                thisRect.position = FallingThreatScript.FallingThreatScripts[targetIdx].GetComponent<RectTransform>().position;
            } else
            {
                TurretScript.autoFire = false;
            }
        }
    }
}
