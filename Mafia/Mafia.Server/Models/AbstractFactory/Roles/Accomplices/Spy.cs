using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Accomplices
{
    public class Spy : Accomplice
    {
        public Spy(int abilityUses = 10)
        {
            Name = "Spy";
            Ability = "At night, learn the role of a chosen player";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new SpyAction();
            RoleAlgorithmPoisoned = new SpyAction();
        }
    }
}
