namespace KioskGame.Service;

public class PlayerStatusDto
{
    public required string PlayerId { get; set; }
    public int PlaysRemaining { get; set; }
    public bool IsSessionExpired { get; set; }
    public DateTime? SessionExpires { get; set; }

    public string? ExpirationReason { get; set; }
}
