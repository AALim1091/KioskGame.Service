using System.Text.Json.Serialization;

namespace KioskGame.Service;

public class PlayResponseDto
{
    public required string PlayerId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PrizeType Prize { get; set; }
    public int PlaysRemaining { get; set; }
    public bool IsSessionExpired { get; set; }
    public DateTime? SessionExpires { get; set; }
    public string? ExpirationReason { get; set; }
}
