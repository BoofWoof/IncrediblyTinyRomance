using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerMenuScript : MonoBehaviour
{
    public Camera TDCamera;
    public GameObject TDPlane;

    public GameObject PhoneTarget;
    public GameObject ScreenTarget;

    public float menuRaiseHeight = -241;
    public float menuLoweredHeight = -434;

    public List<GameObject> towersList;

    private bool raised = false;

    PlayerControls inputs;
    public void BeginTowerPlace(int towerIdx)
    {
        LowerMenu();
        StartCoroutine(TowerPlaceCoroutine(towerIdx));
    }

    public IEnumerator TowerPlaceCoroutine(int towerIdx)
    {
        GameObject newTower = Instantiate(towersList[towerIdx]);
        while (true)
        {
            yield return null;
            // Check for right click
            if (Input.GetMouseButtonDown(1) || inputs.Overworld.TogglePhone.WasPressedThisFrame())
            {
                Destroy(newTower);
                Debug.Log("Breaking");
                yield break;
            }
            // Cast a ray from the camera to the mouse position

            //Plane
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject != PhoneTarget) continue;

                RectTransform rectTransform = ScreenTarget.GetComponent<RectTransform>();
                Vector3 localHitPoint = rectTransform.InverseTransformPoint(hit.point);

                // Calculate the pixel coordinates
                Vector2 imageSize = rectTransform.sizeDelta; // Width and Height of the image in pixels
                Vector2 pixelCoordinates = new Vector2(
                    (localHitPoint.x + (imageSize.x / 2)) / imageSize.x, // Normalize X
                    (localHitPoint.y + (imageSize.y / 2)) / imageSize.y  // Normalize Y
                );

                // Convert normalized coordinates to pixel coordinates
                int pixelX = Mathf.FloorToInt(pixelCoordinates.x * (1280f));
                int pixelY = Mathf.FloorToInt(pixelCoordinates.y * (720f));

                Vector3 localMousePosition = new Vector3(pixelX, pixelY, 0);

                Plane targetPlane = new Plane(Vector3.up, TDPlane.transform.position);

                ray = TDCamera.ScreenPointToRay(localMousePosition);
                float distance;

                // Perform the raycast
                if (targetPlane.Raycast(ray, out distance))
                {
                    Vector3 hitPoint = ray.GetPoint(distance);
                    newTower.transform.position = hitPoint - Vector3.up * 3.5f;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    newTower.GetComponent<TDTowerScript>().ActivateTower();
                    yield break;
                }
            }
        }
    }

    private void Start()
    {
        inputs = new PlayerControls();
        inputs.Enable();
        LowerMenu();
    }

    public void toggleMenu()
    {
        if (raised)
        {
            LowerMenu();
        } else
        {
            RaiseMenu();
        }
    }

    public void RaiseMenu()
    {
        transform.localPosition = Vector3.up * menuRaiseHeight;
        GetComponentInChildren<TextMeshProUGUI>().text = "vvv";
        raised = true;
    }
    public void LowerMenu()
    {
        transform.localPosition = Vector3.up * menuLoweredHeight;
        GetComponentInChildren<TextMeshProUGUI>().text = "^^^";
        raised = false;
    }
}
