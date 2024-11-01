using Mafia.Server.Models.Prototype;

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
    }
}
