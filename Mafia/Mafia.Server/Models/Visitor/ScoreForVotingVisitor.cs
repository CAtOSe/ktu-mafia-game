using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.Visitor;
using Mafia.Server.Models;

public class ScoreForVotingVisitor : IScoreVisitor
{
    private int _correctVotePoints = 15; // Points for voting off a killer

    public void VisitKiller(Killer killer, Player player)
    {
    }

    public void VisitCitizen(Citizen citizen, Player player)
    {
        if (player.CurrentVote != null && player.CurrentVote.RoleType == "Killer")
        {
            player.Score += _correctVotePoints;
            Console.WriteLine($"{player.Name} the ({citizen.Name}) gained {_correctVotePoints} points for voting off a killer.");
        }
    }

    public void VisitAccomplice(Accomplice accomplice, Player player)
    {
    }
}
