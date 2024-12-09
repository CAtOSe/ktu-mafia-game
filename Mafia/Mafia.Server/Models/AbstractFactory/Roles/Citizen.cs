using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Visitor;

namespace Mafia.Server.Models.AbstractFactory.Roles
{
    public class Citizen : Role
    {
        public Citizen()
        {
            RoleType = "Citizen";
            Alignment = "Good";
            Goal = "You win if Killer dies";
        }

        public override IRolePrototype Clone()
        {
            return (IRolePrototype)this.MemberwiseClone();
        }
        public override void Accept(IScoreVisitor visitor, Player player)
        {
            visitor.VisitCitizen(this, player);
        }
    }
}
