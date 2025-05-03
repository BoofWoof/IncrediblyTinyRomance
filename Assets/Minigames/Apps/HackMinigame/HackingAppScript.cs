using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HackingAppScript : MonoBehaviour
{
    public HackingLevelSO CurrentLevel;

    public GameObject EnemyBG1Parent;
    private static GameObject EnemyBG1;
    public GameObject EnemyBG2Parent;
    private static GameObject EnemyBG2;

    private static GameObject Enemy;

    public GameObject MenuElements;
    private static GameObject MenuElementsStatic;

    public void Start()
    {
        MenuElementsStatic = MenuElements;
        ResetLevel();
    }

    public void StartSong()
    {
        MusicSelectorScript.SetPhoneSong(2);
    }
    public void EndSong()
    {
        MusicSelectorScript.SetPhoneSong(MusicSelectorScript.instance.DefaultStartSongPhoneID);
    }

    private void OnEnable()
    {
        GetComponent<AppScript>().OnShowApp += StartSong;
        GetComponent<AppScript>().OnHideApp += EndSong;
    }

    private void OnDisable() 
    {

        GetComponent<AppScript>().OnShowApp -= StartSong;
        GetComponent<AppScript>().OnHideApp -= EndSong;
    }

    public void StartLevel()
    {
        ResetLevel();
        MakeBackground(CurrentLevel.EnemyBG);
        MakeEnemy(CurrentLevel.Enemy);
        MenuElementsStatic.SetActive(false);
        CombatantScript.StartAllCombats();
    }

    public static void ResetLevel()
    {
        ScreenHealthScript.Reset();

        if (EnemyBG1)
        {
            Destroy(EnemyBG1);
        }
        if (EnemyBG2)
        {
            Destroy(EnemyBG2);
        }
        if (Enemy)
        {
            Destroy(Enemy);
        }
        MenuElementsStatic.SetActive(true);
        CombatantScript.StopAllCombats();
        EventSystem.current.SetSelectedGameObject(null);
        CombatantScript.PlayerCombatant.GetComponent<Animator>().SetTrigger("Reset");
    }

    public void MakeBackground(GameObject bgObject)
    {
        EnemyBG1 = Instantiate(bgObject);
        EnemyBG1.transform.parent = EnemyBG1Parent.transform;
        EnemyBG1.transform.localPosition = Vector3.zero;
        EnemyBG1.transform.localRotation = Quaternion.identity;
        EnemyBG1.transform.localScale = Vector3.one;

        EnemyBG2 = Instantiate(bgObject);
        EnemyBG2.transform.parent = EnemyBG2Parent.transform;
        EnemyBG2.transform.localPosition = Vector3.zero;
        EnemyBG2.transform.localRotation = Quaternion.identity;
        EnemyBG2.transform.localScale = Vector3.one;
    }

    public void MakeEnemy(GameObject EnemyObject)
    {
        Enemy = Instantiate(EnemyObject);
        Enemy.transform.parent = transform;
        Enemy.transform.localPosition = Vector3.zero;
        Enemy.transform.localRotation = Quaternion.identity;
        Enemy.transform.localScale = Vector3.one;
    }
}
