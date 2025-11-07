
public static class GameData
{
}

public static class CurrencyData
{
    public static float Credits = 0;
    public static float RenownFlock = 0;
    public static float RenownAscension = 0;
    public static float RenownFoundation = 0;
    public static float RenownRevolution = 0;
}

public static class CurrencyGet
{
    public static ref float GetRenown(StockNames stockNames)
    {
        if(stockNames == StockNames.Flock)
        {
            return ref CurrencyData.RenownFlock;
        }
        if (stockNames == StockNames.Assscensssion)
        {
            return ref CurrencyData.RenownAscension;
        }
        if (stockNames == StockNames.Revolution)
        {
            return ref CurrencyData.RenownRevolution;
        }
        if (stockNames == StockNames.Foundation)
        {
            return ref CurrencyData.RenownFoundation;
        }
        return ref CurrencyData.RenownFlock;
    }
}

public static class TurkData
{
    public static int PuzzlesSolved = 0;
    public static float CreditsPerPuzzle = 0.000_000_001f;
    public static float VisionStrength = 0.9f;//0.1f;
}

public static class GameConstants
{
    public static int CreditIdx = 1;
    public static int FlockRenownIdx = 0;
    public static int AssscensssionRenownIdx = 2;
    public static int FoundationRenownIdx = 5;
    public static int RevolutionRenownIdx = 4;
}
