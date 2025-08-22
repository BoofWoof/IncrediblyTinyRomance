using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class TurretScript : MonoBehaviour
{
    public RectTransform canvasSpace;
    public RectTransform target;
    public RectTransform gun;

    public RectTransform bulletParent;

    public GameObject blastObject;

    // Update is called once per frame
    void Update()
    {

        // Get direction from turret to mouse (in local canvas space)
        Vector2 canvasGunPos = (Vector2)canvasSpace.InverseTransformPoint(gun.position);

        Vector2 dir = (Vector2)target.localPosition - canvasGunPos;

        // Calculate angle (atan2 gives radians, convert to degrees)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Apply rotation (z-axis since it’s 2D UI element)
        gun.localRotation = Quaternion.Euler(0, 0, angle - 90f);

        if (Input.GetMouseButtonDown(0))
        {
            float x = target.localPosition.x;
            if (ADTargetScript.ValidTarget)
            {
                GameObject newBlast = Instantiate(blastObject, bulletParent);
                RectTransform newRectTransform = newBlast.GetComponent<RectTransform>();
                newRectTransform.position = gun.position;
                newRectTransform.localRotation = gun.localRotation;
                newRectTransform.SetSiblingIndex(0);

                DiageticTurretScript.FireAll();
            }
        }
    }
}
