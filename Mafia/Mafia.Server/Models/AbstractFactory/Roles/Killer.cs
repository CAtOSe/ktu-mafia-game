using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Visitor;

namespace Mafia.Server.Models.AbstractFactory.Roles
{
    public class Killer : Role
    {
        public Killer()
        {
            RoleType = "Killer";
            Alignment = "Evil";
            Goal = "You win if you remain alive and the evil team has majority";
        }

        public override IRolePrototype Clone()
        {
            return (IRolePrototype)this.MemberwiseClone();
        }

        public override void Accept(IScoreVisitor visitor, Player player)
        {
            visitor.VisitKiller(this, player);
        }
    }
}
