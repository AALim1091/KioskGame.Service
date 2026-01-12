namespace KioskGame.Service;

public class PlayHistory
{
    public Guid Id { get; set; }
    public required string PlayerId { get; set; }
    public DateTime LastPlayed { get; set; }
    public PrizeType PrizeAwarded { get; set; }
}
