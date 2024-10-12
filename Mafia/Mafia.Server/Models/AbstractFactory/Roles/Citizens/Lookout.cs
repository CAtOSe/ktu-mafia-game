namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Lookout : Citizen
    {
        public Lookout(int abilityUses = 10)
        {
            Name = "Lookout";
            Ability = "At night, choose a player to see who visited them during the night";
            AbilityUsesLeft = abilityUses;
        }
    }
}
