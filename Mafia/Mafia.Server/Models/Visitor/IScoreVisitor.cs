using Mafia.Server.Models.AbstractFactory.Roles;

namespace Mafia.Server.Models.Visitor
{
    public interface IScoreVisitor
    {
        void VisitKiller(Killer killer, Player player);
        void VisitCitizen(Citizen citizen, Player player);
        void VisitAccomplice(Accomplice accomplice, Player player);

    }
}
