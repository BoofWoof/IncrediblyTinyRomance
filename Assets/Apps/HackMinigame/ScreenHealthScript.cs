using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenHealthScript : MonoBehaviour
{
    public RectMask2D BGMask1;
    public RectMask2D BGMask2;
    public GameObject BGMaskParticles;
    public static float HealthBalance = 0;
    public float ParticleWidth = 0.53f;

    public float BarSpeedMult = 2f;
    public float MinBarSpeed = 0.5f;
    public static float BarPosition = 0;

    public static bool CombatRunning = false;

    private void Start()
    {
        StartCoroutine(BarUpdate());
    }

    IEnumerator BarUpdate()
    {
        while (true)
        {
            float barspeed = Mathf.Abs(BarPosition - HealthBalance) * BarSpeedMult;
            barspeed = Mathf.Max(barspeed, MinBarSpeed);
            BarPosition = Mathf.MoveTowards(BarPosition, HealthBalance, barspeed * Time.deltaTime);

            BGMask1.padding = new Vector4(800 + BarPosition * 800, 0, 0, 0);
            BGMask2.padding = new Vector4(0, 0, 800 - BarPosition * 800, 0);
            BGMaskParticles.transform.localPosition = new Vector3(ParticleWidth * BarPosition, 0, 0.891f);
            yield return null;
        }
    }

    private void Update()
    {
        if (CombatRunning)
        {
            if (BarPosition > 1f)
            {
                HealthBalance = 2f;
                BarPosition = 2f;
                CombatantScript.PlayerCombatant.GetComponent<Animator>().SetTrigger("Win");
                CombatantScript.EnemyCombatant.GetComponent<Animator>().SetTrigger("Die");
                CombatRunning = false;
                CombatantScript.StopAllCombats();
            }
            if (BarPosition < -1f)
            {
                HealthBalance = -2f;
                BarPosition = -2f;
                CombatantScript.PlayerCombatant.GetComponent<Animator>().SetTrigger("Die");
                CombatRunning = false;
                CombatantScript.StopAllCombats();
            }
        }
    }
    public static void Reset()
    {
        HealthBalance = 0;
        BarPosition = 0;
        CombatRunning = true;
    }
}
