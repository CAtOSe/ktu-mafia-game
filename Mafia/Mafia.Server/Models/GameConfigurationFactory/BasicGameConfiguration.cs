using Mafia.Server.Models;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server;

public class BasicGameConfiguration : IGameConfiguration
{
    public int BeginCountdown => 5000;
    public int DayPhaseDuration => 10 * 1000;
    public int NightPhaseDuration => 20 * 1000;
    
    public static List<string> ActionOrder = new()
    {
        nameof(PoisonerAction),
        nameof(TrackerAction),
        nameof(AssassinAction),
        nameof(SoldierAction),
        nameof(DoctorAction)
    };
}