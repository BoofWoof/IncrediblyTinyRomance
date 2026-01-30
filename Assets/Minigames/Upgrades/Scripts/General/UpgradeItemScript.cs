using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemScript : MonoBehaviour
{
    public UpgradesAbstract AssociatedUpgrade;
    public UpgradeScreenScript SourceScreen;

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public TextMeshProUGUI CostText;
    public Image UpgradeImage;

    public float Duration = 1f;
    public static int UpgradesAnimating = 0;

    public void UpdateUI()
    {
        NameText.text = AssociatedUpgrade.UpgradeName;
        DescriptionText.text = AssociatedUpgrade.UpgradeDescription;
        CostText.text = AssociatedUpgrade.CostToText();
        UpgradeImage.sprite = AssociatedUpgrade.UpgradeIcon;
    }

    public void SetUpgrade(UpgradesAbstract associatedUpgrade)
    {
        AssociatedUpgrade = associatedUpgrade;
        UpdateUI();
    }

    public void SetSource(UpgradeScreenScript sourceScreen)
    {
        SourceScreen = sourceScreen;
    }

    public void Buy()
    {
        if (!AssociatedUpgrade.Buy()) {
            AnnouncementScript.StartAnnouncement("You can't afford this upgrade. Go do more puzzles!");
            return;
        } 
        StartCoroutine(UpgradeBoughtAnimation());
    }

    public void Start()
    {
        StartCoroutine(AffordCheck());
    }

    public IEnumerator AffordCheck()
    {
        if (AssociatedUpgrade.CanBuy()) UpgradeImage.color = new Color(1f, 1f, 1f);
        else UpgradeImage.color = new Color(0.35f, 0.3f, 0.3f);
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator UpgradeBoughtAnimation()
    {
        UpgradesAnimating++;
        Image image = GetComponent<Image>();
        Material runtimeMaterial = Instantiate(image.material);
        image.material = runtimeMaterial;

        float elapsed = 0f;
        while (elapsed < Duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / Duration);
            float progress = Mathf.Lerp(0.3f, 1f, t);
            image.materialForRendering.SetFloat("_Disappear", progress);
            yield return null;
        }

        // Ensure it ends at 1
        image.materialForRendering.SetFloat("_Disappear", 1f);

        UpgradesAnimating--;

        if(UpgradesAnimating == 0)
        {
            SourceScreen.Refresh();
        }
    }

}