namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Doctor : Citizen
    {
        public Doctor(int abilityUses = 10)
        {
            Name = "Doctor";
            Ability = "At night, choose a player to protect them from dying";
            AbilityUsesLeft = abilityUses;
        }

        public override void NightAction(Player user, Player target, List<NightAction> nightActions)
        {
            if (!target.IsAlive)
            {
                target.IsAlive = true;
                string messageToTarget = "You were attacked by the Killer, but survived due to Doctor's help!"; // Death Message
                //Killer You attacked X at night, but they survived the attack.
            }

            string messageToUser = "You protected " + target.Name + " tonight.";
            //Kitoks jeigu issaugo?

            Console.Write(messageToUser);
            
        }
    }
}
