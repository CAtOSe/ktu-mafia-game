namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Tracker : Citizen
    {
        public Tracker(int abilityUses = 10)
        {
            Name = "Tracker";
            Ability = "At night, choose a player to find out who they visited that night";
            AbilityUsesLeft = abilityUses;
        }
    }
}
