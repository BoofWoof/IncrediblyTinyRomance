using UnityEngine;

[CreateAssetMenu(fileName = "UnlockStockUpgrade", menuName = "Upgrades/UnlockStockUpgrade")]
public class UnlockStockUpgrade : UpgradesAbstract
{
    public StockNames UnlockStock;

    public override void OnBuy()
    {
        StockUnlockScript.instance.UnlockStock(UnlockStock);
    }
}
