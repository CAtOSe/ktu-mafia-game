namespace Mafia.Server.Models.AbstractFactory.Roles.Killers
{
    public class Assassin : Killer
    {
        public Assassin(int abilityUses = 10)
        {
            Name = "Assassin";
            Ability = "At night, kill a player";
            AbilityUsesLeft = abilityUses;
        }

        public override void NightAction(Player user, Player target, List<NightAction> nightActions)
        {
            target.IsAlive = false;

            string messageToUser = "You killed " + target.Name + " tonight.";
            string messageToTarget = "You were attacked by the Killer and died."; // Death Message

            Console.Write(messageToUser);
        }
    }
}
