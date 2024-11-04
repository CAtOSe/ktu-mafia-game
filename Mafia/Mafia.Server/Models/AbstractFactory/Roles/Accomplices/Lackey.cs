using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Accomplices
{
    public class Lackey : Accomplice
    {
        public Lackey(int abilityUses = 0)
        {
            Name = "Lackey";
            Ability = "You have no ability";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new NoAction();
            RoleAlgorithmPoisoned = new NoAction();
        }

        public override IRolePrototype Clone()
        {
            var clone = (Lackey)this.MemberwiseClone();
            return clone;
        }

    }
}
