namespace Mafia.Server.Models.AbstractFactory.Roles.Accomplices
{
    public class Poisoner : Accomplice
    {
        public Poisoner(int abilityUses = 10)
        {
            Name = "Poisoner";
            Ability = "At night, poison a player - their ability does not work, if ability provides information - they get wrong information";
            AbilityUsesLeft = abilityUses;
        }
    }
}
