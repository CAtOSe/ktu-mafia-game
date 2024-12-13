using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.Visitor;
using Mafia.Server.Models;

public class ScoreForSurvivingVisitor : IScoreVisitor
{
    private int _daysSurvivedPoints = 10; // Points for surviving a day

    public void VisitKiller(Killer killer, Player player)
    {
        player.Score += _daysSurvivedPoints;
        Console.WriteLine($"{player.Name} the ({killer.Name}) gained {_daysSurvivedPoints} points for surviving the day.");
    }

    public void VisitCitizen(Citizen citizen, Player player)
    {
        player.Score += _daysSurvivedPoints/2;
        Console.WriteLine($"{player.Name} the ({citizen.Name}) gained {_daysSurvivedPoints/2} points for surviving the day.");
    }

    public void VisitAccomplice(Accomplice accomplice, Player player)
    {
        player.Score += _daysSurvivedPoints;
        Console.WriteLine($"{player.Name} the ({accomplice.Name}) gained {_daysSurvivedPoints} points for surviving the day.");
    }
}
