namespace KioskGame.Service;

public class SessionService
{
    public const int MaxPlaysPerDay = 3;
    public static readonly TimeSpan SessionWindow = TimeSpan.FromMinutes(5);

    private readonly IClock _clock;

    public SessionService(IClock clock)
    {
        _clock = clock;
    }

    public DateTime TodayUtc()
    {
        var now = DateTime.UtcNow;
        return new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public bool IsExpired(DateTime sessionStartUtc) => _clock.UtcNow > sessionStartUtc.Add(SessionWindow);

    public DateTime ExpiresAt(DateTime sessionStartUtc)  => sessionStartUtc.Add(SessionWindow);

    public int PlaysRemaining(int playsUsed) => Math.Max(0, MaxPlaysPerDay - playsUsed);
}
