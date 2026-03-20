using PixelCrushers;
using System;

public class CurrencySaver : Saver
{
    //CURRENTLY ONLY SAVES CREDITS, ADD OTHER CURRENCIES LATER
    [Serializable]
    public class CurrencySaveData 
    {
        public float Credits;
    }
    public override string RecordData()
    {
        CurrencySaveData newSaveData = new CurrencySaveData()
        {
            Credits = CurrencyData.Credits
        };
        return SaveSystem.Serialize(newSaveData);
    }
    public override void ApplyData(string s)
    {
        CurrencySaveData saveData = SaveSystem.Deserialize<CurrencySaveData>(s);

        if (saveData == null) return;

        CurrencyData.Credits = saveData.Credits;
    }
}
