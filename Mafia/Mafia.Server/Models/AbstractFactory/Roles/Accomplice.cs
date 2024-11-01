using Mafia.Server.Models.Prototype;

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
    }
}
