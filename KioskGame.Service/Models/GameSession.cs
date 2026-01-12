namespace KioskGame.Service;

public class GameSession
{
    public Guid Id { get; set; }
    public required string PlayerId { get; set; }
    public DateTime Today { get; set; }
    public DateTime? SessionStartTime { get; set; }
    public int PlaysUsed { get; set; }
}
