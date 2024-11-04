namespace Mafia.Server.Models;

public interface IGameConfiguration
{
    public int BeginCountdown { get; }
    public int DayPhaseDuration { get; }
    public int NightPhaseDuration { get; }
}