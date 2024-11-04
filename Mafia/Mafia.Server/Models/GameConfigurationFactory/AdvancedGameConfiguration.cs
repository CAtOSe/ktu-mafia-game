namespace Mafia.Server.Models.GameConfigurationFactory;

public class AdvancedGameConfiguration : IGameConfiguration
{
    public int BeginCountdown => 3 * 1000;
    public int DayPhaseDuration => 5 * 1000;
    public int NightPhaseDuration => 10 * 1000;
}