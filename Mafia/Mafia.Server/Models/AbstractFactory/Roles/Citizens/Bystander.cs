namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Bystander : Citizen
    {
        public Bystander(int abilityUses = 0)
        {
            Name = "Bystander";
            Ability = "You have no ability";
            AbilityUsesLeft = abilityUses;
        }
    }
}
