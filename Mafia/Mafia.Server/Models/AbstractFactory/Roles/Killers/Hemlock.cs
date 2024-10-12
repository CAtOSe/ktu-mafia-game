namespace Mafia.Server.Models.AbstractFactory.Roles.Killers
{
    public class Hemlock : Killer
    {
        public Hemlock(int abilityUses = 10)
        {
            Name = "Hemlock";
            Ability = "At night, poison a player, they die the next night";
            AbilityUsesLeft = abilityUses;
        }
    }
}
