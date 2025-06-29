using TMPro;
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
        if (!AssociatedUpgrade.Buy()) return;
        SourceScreen.Refresh();
    }
}