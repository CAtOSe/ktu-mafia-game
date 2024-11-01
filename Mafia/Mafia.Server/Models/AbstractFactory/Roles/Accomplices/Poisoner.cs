using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Accomplices
{
    public class Poisoner : Accomplice
    {
        public Poisoner(int abilityUses = 10)
        {
            Name = "Poisoner";
            Ability = "At night, poison a player - their ability does not work, if ability provides information - they get wrong information";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new PoisonerAction();
            RoleAlgorithmPoisoned = new PoisonerAction();
        }

        public override IRolePrototype Clone()
        {
            var clone = (Poisoner)this.MemberwiseClone();
            return clone;
        }
    }
}
