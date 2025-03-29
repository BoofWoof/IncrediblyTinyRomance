using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GachaBagScript : MonoBehaviour
{
    public Animator BagAnimator;
    public Button ResearchButton;

    public List<HackingEquipmentSO> CommonEquipmentPool;
    private HackingEquipmentSO newEquipment;

    public Image Prize;

    public void Start()
    {
        ResetBag();
    }

    public void SummonItem()
    {
        int sample_idx = Random.Range(0, CommonEquipmentPool.Count);
        newEquipment = Instantiate(CommonEquipmentPool[sample_idx]);
        newEquipment.RandomizeValues();
        Prize.gameObject.SetActive(true);

        Prize.sprite = newEquipment.EquipmentSprite;
        Prize.SetNativeSize();
    }

    public void OpenBag()
    {
        BagAnimator.SetTrigger("Open");
        if(EventSystem.current.currentSelectedGameObject == ResearchButton.gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        ResearchButton.gameObject.SetActive(false);

        StartCoroutine(ResetWait());
    }

    public IEnumerator ResetWait()
    {
        yield return new WaitForSeconds(3f);
        ResetBag();
    }

    public void ResetBag()
    {
        BagAnimator.SetTrigger("Reset");

        ResearchButton.gameObject.SetActive(true);

        Prize.gameObject.SetActive(false);

        if(newEquipment != null)
        {
            HEquipmentManagerScript.EquipmentManager.AssignNewEquipment(newEquipment);
            Destroy(newEquipment);
        }
    }
}
