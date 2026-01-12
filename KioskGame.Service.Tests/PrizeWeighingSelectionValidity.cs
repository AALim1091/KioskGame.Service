namespace KioskGame.Service.Tests;

public class PrizeWeighingSelectionValidity
{
    [Fact]
    public void Roll_Produces_All_Prize_Types_With_Weighting()
    {
        var service = new PrizeService();
        var results = new Dictionary<PrizeType, int>();

        foreach (PrizeType p in Enum.GetValues(typeof(PrizeType)))
            results[p] = 0;

        for (int i = 0; i < 10000; i++)
        {
            results[service.Spin()]++;
        }

        Assert.True(results[PrizeType.NoPrize] > results[PrizeType.GiftItem]);
        Assert.True(results[PrizeType.GiftItem] > 0);
    }
}
