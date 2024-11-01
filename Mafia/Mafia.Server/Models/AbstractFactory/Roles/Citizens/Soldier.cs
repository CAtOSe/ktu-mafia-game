using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Soldier : Citizen
    {
        public Soldier(int abilityUses = 10)
        {
            Name = "Soldier";
            Ability = "At night, you may protect yourself from dying";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new SoldierAction();
            RoleAlgorithmPoisoned = new SoldierActionPoisoned();
        }

        public override IRolePrototype Clone()
        {
            var clone = (Soldier)this.MemberwiseClone();
            return clone;
        }

        /*
        public override void NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            string messageToUser = "You used your shield to protect yourself tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);

            if (!target.IsAlive)
            {
                target.IsAlive = true;
                string messageToTarget = "Your shield protected you from death.";
                ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
                nightMessages.Add(chatMessageToTarget);
            }
        }*/
    }
}
