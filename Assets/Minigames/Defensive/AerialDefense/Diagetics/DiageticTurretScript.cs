using System.Collections.Generic;
using UnityEngine;

public class DiageticTurretScript : MonoBehaviour
{
    public GameObject TurretTop;
    public GameObject TurretBase;

    public RectTransform AngleSource;

    public GameObject BulletSource;
    public GameObject BulletPrefab;
    public GameObject BulletParent;

    public static List<DiageticTurretScript> TurretScripts = new List<DiageticTurretScript>();

    private void Start()
    {
        TurretScripts.Add(this);
    }

    private void OnDestroy()
    {
        TurretScripts.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 TopOriginalEuler = TurretTop.transform.rotation.eulerAngles;
        TurretTop.transform.rotation = Quaternion.Euler(90f * AngleSource.localPosition.x/960f, TopOriginalEuler.y, TopOriginalEuler.z);
        Vector3 BaseOriginalEuler = TurretBase.transform.rotation.eulerAngles;
        TurretBase.transform.rotation = Quaternion.Euler(BaseOriginalEuler.x, 180f * AngleSource.localPosition.y / 1080f, BaseOriginalEuler.z);
    }

    public static void FireAll()
    {
        foreach(DiageticTurretScript tScript in TurretScripts)
        {
            tScript.Fire();
        }
    }

    public void Fire()
    {
        GameObject newGameObject = Instantiate(BulletPrefab);
        newGameObject.transform.parent = BulletParent.transform;
        newGameObject.transform.position = BulletSource.transform.position;
        newGameObject.transform.rotation = BulletSource.transform.rotation;
        newGameObject.transform.localScale = new Vector3(0.2f, 1.5f, 0.2f);
    }
}
