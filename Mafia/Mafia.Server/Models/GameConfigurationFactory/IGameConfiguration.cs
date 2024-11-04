namespace Mafia.Server.Models.GameConfigurationFactory;

public interface IGameConfiguration
{
    public int BeginCountdown { get; }
    public int DayPhaseDuration { get; }
    public int NightPhaseDuration { get; }
}