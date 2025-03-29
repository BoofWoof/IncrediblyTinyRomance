using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CombatantScript : MonoBehaviour
{
    public static CombatantScript PlayerCombatant;
    public static CombatantScript EnemyCombatant;

    public bool isPlayerCombatant = false;

    public float BaseAttack = 0f;
    public float BaseAttackRate = 0.33f;
    public float BaseDefense = 0f;

    public static List<CombatantScript> allCombatants = new List<CombatantScript>();
    private Coroutine CurrentCoroutineAction = null;

    private void Awake()
    {
        if (isPlayerCombatant)
        {
            PlayerCombatant = this;
        } else
        {
            EnemyCombatant = this;
        }
    }

    private void OnEnable()
    {
        allCombatants.Add(this);
    }

    private void OnDisable()
    {
        allCombatants.Remove(this);
    }

    public static void StartAllCombats()
    {
        foreach (CombatantScript instance in allCombatants)
        {
            instance.StartCombat();
        }
    }
    public static void StopAllCombats()
    {
        foreach (CombatantScript instance in allCombatants)
        {
            instance.StopCombat();
        }
    }

    void StopCombat()
    {
        if(CurrentCoroutineAction != null)
        {
            StopCoroutine(CurrentCoroutineAction);
        }
    }

    void StartCombat()
    {
        CurrentCoroutineAction = StartCoroutine(AttackLoop());
    }

    public void Attack(float Damage)
    {
        if (isPlayerCombatant)
        {
            float DamagePercent = 100f / (100f + EnemyCombatant.BaseDefense);
            ScreenHealthScript.HealthBalance += BaseAttack * DamagePercent;
        } else
        {
            float DamagePercent = 100f / (100f + PlayerCombatant.BaseDefense);
            ScreenHealthScript.HealthBalance -= Damage * DamagePercent;
        }
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            GetComponent<Animator>().SetTrigger("Attack");
            yield return null;
            GetComponent<Animator>().ResetTrigger("Attack");
            yield return new WaitForSeconds(1f / BaseAttackRate);
        }
    }
}
