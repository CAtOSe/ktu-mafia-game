using Mafia.Server.Models.Strategy;

namespace Mafia.Server;

public static class GameConfiguration
{
    public const int BeginCountdown = 5000;
    public const int DayPhaseDuration = 10 * 1000;
    public const int NightPhaseDuration = 20 * 1000;
    
    public static List<string> ActionOrder = new()
    {
        nameof(PoisonerAction),
        nameof(TrackerAction),
        nameof(AssassinAction),
        nameof(SoldierAction),
        nameof(DoctorAction)
    };
}