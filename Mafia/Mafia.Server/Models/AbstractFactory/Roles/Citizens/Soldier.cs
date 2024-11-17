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

        // TEMPLATE METHOD hook
        protected override bool NeedBeforeAction()
        {
            if (AbilityUsesLeft <= 0)
            {
                return false;
            }
            return true;
        }

        // TEMPLATE METHOD hook
        protected override bool NeedAfterAction(Player target)
        {
            if (target != null && !target.IsAlive)
            {
                return true;
            }
            return false;
        }

        // TEMPLATE METHOD: Pre-action
        protected override Task BeforeAction(Player user, Player target, List<ChatMessage> messages)
        {
            string text = $"Your action target: {target.Name}.";
            messages.Add(new ChatMessage("", $"TEMPLATE METHOD: {text}", user.Name, "nightNotification"));
            Console.WriteLine($"TEMPLATE METHOD: Before action of {Name}");
            return Task.CompletedTask;
        }

        // TEMPLATE METHOD: Post-action
        protected override Task AfterAction(Player user, Player target, List<ChatMessage> messages)
        {
            target.IsAlive = true;
            AbilityUsesLeft--;
            messages.Add(new ChatMessage("", "TEMPLATE METHOD: Your shield protected you from an attack!",
                user.Name, "nightNotification"));
            
            Console.WriteLine($"TEMPLATE METHOD: After action of {Name}");
            return Task.CompletedTask;
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
