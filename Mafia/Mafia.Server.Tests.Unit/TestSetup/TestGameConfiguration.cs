using Mafia.Server.Models.GameConfigurationFactory;

namespace Mafia.Server.Tests.Unit.TestSetup;

public class TestGameConfiguration : IGameConfiguration
{
    public int BeginCountdown => TestData.BeginCountdown;
    public int DayPhaseDuration => TestData.DayPhaseDuration;
    public int NightPhaseDuration => TestData.NightPhaseDuration;
}