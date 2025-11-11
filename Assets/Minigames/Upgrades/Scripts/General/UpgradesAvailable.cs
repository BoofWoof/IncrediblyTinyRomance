using UnityEngine;

public class UpgradesAvailable : MonoBehaviour
{
    public GameObject ButtonGlow;

    public UpgradeScreenScript UpgradeScreen;

    public void Update()
    {
        if (ButtonGlow == null) return;
        bool anyUpgradePurchasable = false;
        foreach (GameObject upgradeObject in UpgradeScreen.UpgradeObjects)
        {
            if (upgradeObject.GetComponent<UpgradeItemScript>().AssociatedUpgrade.CanBuy())
            {
                anyUpgradePurchasable = true;
            }
        }
        ButtonGlow.SetActive(anyUpgradePurchasable);
    }
}
