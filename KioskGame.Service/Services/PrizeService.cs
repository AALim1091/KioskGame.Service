namespace KioskGame.Service;

public class PrizeService
{
    private readonly Dictionary<PrizeType, int> _weights = new()
{
    { PrizeType.NoPrize, 50 },
    { PrizeType.FreePlay5, 25 },
    { PrizeType.FreePlay10, 15 },
    { PrizeType.FoodVoucher, 7 },
    { PrizeType.GiftItem, 3 }
};

    public PrizeType Spin()
    {
        int sumOfWeights = _weights.Values.Sum();
        int randomNumber = Random.Shared.Next(sumOfWeights);

        foreach (var entry in _weights)
        {
            int weight = entry.Value;

            if (randomNumber < weight)
                return entry.Key;

            randomNumber -= weight;
        }
        return PrizeType.NoPrize;
    }
}
