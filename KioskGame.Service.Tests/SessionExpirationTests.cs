namespace KioskGame.Service.Tests;

public class SessionExpirationTests
{
    public class Clock : IClock
    {
        public DateTime UtcNow { get; set; }

        public Clock(DateTime now)
        {
            UtcNow = now;
        }
    }

    [Fact]
    public void IsExpired_BeforeFiveMinutes()
    {
        var clock = new Clock(DateTime.UtcNow);
        var service = new SessionService(clock);

        var start = clock.UtcNow.AddMinutes(-4);

        Assert.False(service.IsExpired(start));
    }

    [Fact]
    public void IsExpired_AfterFiveMinutes()
    {
        var clock = new Clock(DateTime.UtcNow);
        var service = new SessionService(clock);

        var start = clock.UtcNow.AddMinutes(-6);

        Assert.True(service.IsExpired(start));
    }
}