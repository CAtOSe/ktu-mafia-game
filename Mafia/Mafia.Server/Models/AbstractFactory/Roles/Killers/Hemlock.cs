using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Killers
{
    public class Hemlock : Killer
    {
        public Hemlock(int abilityUses = 10)
        {
            Name = "Hemlock";
            Ability = "At night, poison a player for 2 nights, they die the next night";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new HemlockAction();
            RoleAlgorithmPoisoned = new HemlockAction();
        }

        public override IRolePrototype Clone()
        {
            var clone = (Hemlock)this.MemberwiseClone();
            return clone;
        }
    }
}
