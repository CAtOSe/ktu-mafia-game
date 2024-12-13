using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Visitor;

namespace Mafia.Server.Models.AbstractFactory.Roles
{
    public class Accomplice : Role
    {
        public Accomplice()
        {
            RoleType = "Accomplice";
            Alignment = "Evil";
            Goal = "You win if your Killer remains alive and the evil team has majority";
        }

        public override IRolePrototype Clone()
        {
            return (IRolePrototype)this.MemberwiseClone();
        }
        public override void Accept(IScoreVisitor visitor, Player player)
        {
            visitor.VisitAccomplice(this, player);
        }
    }
}
