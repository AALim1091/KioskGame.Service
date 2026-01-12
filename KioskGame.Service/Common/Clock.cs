namespace KioskGame.Service;

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
