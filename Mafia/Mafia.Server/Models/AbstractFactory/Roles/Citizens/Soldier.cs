namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Soldier : Citizen
    {
        public Soldier(int abilityUses = 10)
        {
            Name = "Soldier";
            Ability = "At night, you may protect yourself from dying";
            AbilityUsesLeft = abilityUses;
        }

        public override void NightAction(Player user, Player target, List<NightAction> nightActions)
        {
            if (!target.IsAlive)
            {
                target.IsAlive = true;
                string messageToTarget = "You were attacked by the Killer, but your shield protected you!"; // Death Message
                //Killer You attacked X at night, but they survived the attack.
            }

            string messageToUser = "You used your shield to protect yourself tonight.";
            Console.Write(messageToUser);
        }
    }
}
