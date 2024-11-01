using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Prototype;

namespace Mafia.Server.Models.AbstractFactory.Roles.Killers
{
    public class Phantom : Killer
    {
        public Phantom(int abilityUses = 10)
        {
            Name = "Phantom";
            Ability = "At night, kill a player, their role is not revealed";
            AbilityUsesLeft = abilityUses;
        }

        public override IRolePrototype Clone()
        {
            var clone = (Phantom)this.MemberwiseClone();
            return clone;
        }
    }
}
