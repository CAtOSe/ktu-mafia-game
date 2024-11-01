using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Killers
{
    public class Assassin : Killer
    {
        public Assassin(int abilityUses = 10)
        {
            Name = "Assassin";
            Ability = "At night, kill a player";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new AssassinAction();
            RoleAlgorithmPoisoned = new AssassinAction();
        }

        public override IRolePrototype Clone()
        {
            var clone = (Assassin)this.MemberwiseClone();
            return clone;
        }

        /*
        public override void NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            target.IsAlive = false;

            string messageToUser = "You attacked " + target.Name + " tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);
            
            string messageToTarget = "You were attacked by the Killer."; // Death Message
            ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
            nightMessages.Add(chatMessageToTarget);
        }*/
    }
}
