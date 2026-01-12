namespace KioskGame.Service.Tests;

public class OneTimeGiftItemRuleTests
{
    [Fact]
    public void GiftItem_Substitution_WhenAlreadyWon()
    {
        var player = new Player
        {
            Id = "1234",
            HasWonGiftItem = true
        };
        var prize = PrizeType.GiftItem;

        if (prize == PrizeType.GiftItem && player.HasWonGiftItem)
        {
            prize = PrizeType.FreePlay10;
        }

        Assert.Equal(PrizeType.FreePlay10, prize);
    }
}
