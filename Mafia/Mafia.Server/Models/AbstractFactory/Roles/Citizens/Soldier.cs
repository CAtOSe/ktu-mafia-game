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
        protected override bool HasAbilitiesLeft()
        {
            if (AbilityUsesLeft <= 0)
            {
                return false;
            }
            return true;
        }

        // TEMPLATE METHOD hook
        protected override bool IsTargetAcceptable(Player target)
        {
            if (target != null)
            {
                return true;
            }
            return false;
        }

        // TEMPLATE METHOD: abilities count
        protected override Task ShowAbilitiesCount(Player user, Player target, List<ChatMessage> messages)
        {
            int abilitiesLeft = AbilityUsesLeft - 1;
            string text = $"You have remaining abilities: {abilitiesLeft}.";
            messages.Add(new ChatMessage("", $"TEMPLATE METHOD: {text}", user.Name, "nightNotification"));
            Console.WriteLine($"TEMPLATE METHOD: abilities count was shown to {user.Name} ({Name})");
            return Task.CompletedTask;
        }

        // TEMPLATE METHOD: success message
        protected override Task ShowSuccessMessage(Player user, Player target, List<ChatMessage> messages)
        {
            target.IsAlive = true;
            if (AbilityUsesLeft > 0) AbilityUsesLeft--; // Decrease ability uses
            messages.Add(new ChatMessage("", "TEMPLATE METHOD: Your shield protected you from an attack!",
                user.Name, "nightNotification"));
            
            Console.WriteLine($"TEMPLATE METHOD: success message was shown to {user.Name} ({Name})");
            return Task.CompletedTask;
        }

        // TEMPLATE METHOD: error message
        protected override Task ShowErrorMessage(Player user, List<ChatMessage> messages)
        {
            messages.Add(new ChatMessage("", "TEMPLATE METHOD: You have no remaining abilities!",
                user.Name, "nightNotification"));

            Console.WriteLine($"TEMPLATE METHOD: error message was shown to {user.Name} ({Name})");
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
