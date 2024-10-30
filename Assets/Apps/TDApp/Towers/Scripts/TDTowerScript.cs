using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TDTowerScript : MonoBehaviour
{
    public TDBuildingRestriction BuildingRestrictionScript;
    public TDTowerTargets TargetsScript;

    public float AttackSpeed = 0.25f;

    public bool TowerActivated = false;

    [Header("Gun Info")]
    public GameObject ProjectileSource;
    public GameObject Projectile;
    public float BulletSpeed = 1f;

    private float Cooldown = 0f;

    public void ActivateTower()
    {
        TowerActivated = true;
        transform.parent = TDAppScript.Level.transform;
    }

    public void Update()
    {
        if (!TowerActivated) return;

        if (Cooldown >= 0f) Cooldown -= Time.deltaTime;
        TargetsScript.CheckDestroyed();
        if (TargetsScript.targets.Count > 0)
        {
            StartCoroutine(AttemptAttack(TargetsScript.targets[0]));
        }
    }
    private IEnumerator AttemptAttack(GameObject target)
    {
        if (Cooldown >= 0f) yield break;
        Cooldown = 1f / AttackSpeed;

        float progress = 0;
        Vector3 startingPos = ProjectileSource.transform.position;
        GameObject projectile = Instantiate(Projectile, startingPos, Quaternion.identity);
        while (progress < 1)
        {
            yield return null;
            if(target == null)
            {
                Destroy(projectile);
                yield break;
            }
            progress += Time.deltaTime;
            projectile.transform.position = Vector3.Lerp(startingPos, target.transform.position, progress);
        }
        Destroy(target);
        Destroy(projectile);

        yield return null;
    }
}
