using UnityEngine;
using UnityEngine.UI;

public class TurretScript : MonoBehaviour
{
    public static int ActiveTurrets = 0;
    public static int CurrentFireIdx = 0;
    public static bool TurretFired = false;

    public int ThisTurretIdx;

    public RectTransform canvasSpace;
    public RectTransform target;
    public RectTransform gun;

    public RectTransform bulletParent;

    public GameObject blastObject;

    public Image chargeMeter;
    public float InitialChargePeriod = 1f;
    public static float ChargePeriod;
    public float currentCharge = 0f;

    public DiageticTurretScript diageticTurretScript;

    public void Start()
    {
        ChargePeriod = InitialChargePeriod;
        ThisTurretIdx = ActiveTurrets;
        ActiveTurrets++;

    }

    // Update is called once per frame
    void Update()
    {
        currentCharge += Time.deltaTime;
        float chargePercentage = currentCharge / ChargePeriod;
        if(chargePercentage > 1) chargePercentage = 1;
        chargeMeter.fillAmount = chargePercentage;

        // Get direction from turret to mouse (in local canvas space)
        Vector2 canvasGunPos = (Vector2)canvasSpace.InverseTransformPoint(gun.position);

        Vector2 dir = (Vector2)target.localPosition - canvasGunPos;

        // Calculate angle (atan2 gives radians, convert to degrees)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Apply rotation (z-axis since it’s 2D UI element)
        gun.localRotation = Quaternion.Euler(0, 0, angle - 90f);

        if (Input.GetMouseButtonDown(0) && chargePercentage >= 1f && CurrentFireIdx == ThisTurretIdx && !TurretFired)
        {
            float x = target.localPosition.x;
            if (ADTargetScript.ValidTarget)
            {
                TurretFired = true;
                CurrentFireIdx++;
                CurrentFireIdx = CurrentFireIdx % ActiveTurrets;

                GameObject newBlast = Instantiate(blastObject, bulletParent);
                RectTransform newRectTransform = newBlast.GetComponent<RectTransform>();
                newRectTransform.position = gun.position;
                newRectTransform.localRotation = gun.localRotation;
                newRectTransform.SetSiblingIndex(0);

                if(diageticTurretScript != null) diageticTurretScript.Fire();
                currentCharge = 0;
            }
        }
    }

    private void LateUpdate()
    {
        TurretFired = false;
    }
}
