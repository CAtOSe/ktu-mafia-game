using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Bystander : Citizen
    {
        public Bystander(int abilityUses = 0)
        {
            Name = "Bystander";
            Ability = "You have no ability";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new NoAction();
            RoleAlgorithmPoisoned = new NoAction();
        }

        public override IRolePrototype Clone()
        {
            var clone = (Bystander)this.MemberwiseClone();
            return clone;
        }


        public Bystander DeepCopy()
        {
            return new Bystander
            {
                Name = "Bystander",
                Ability = "You have no ability",
                AbilityUsesLeft = 0
            };
        }
    }
}
