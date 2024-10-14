namespace Mafia.Server.Models.AbstractFactory.Roles.Accomplices
{
    public class Lackey : Accomplice
    {
        public Lackey(int abilityUses = 0)
        {
            Name = "Lackey";
            Ability = "You have no ability";
            AbilityUsesLeft = abilityUses;
        }
    }
}
