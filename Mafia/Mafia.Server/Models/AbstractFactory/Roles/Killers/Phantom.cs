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
    }
}
