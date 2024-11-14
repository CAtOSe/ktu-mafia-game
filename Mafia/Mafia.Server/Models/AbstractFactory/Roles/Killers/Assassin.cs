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

        // TEMPLATE METHOD: Pre-action hook
        protected override Task BeforeAction(Player user, Player target, List<ChatMessage> messages)
        {
            string text = string.Empty;
            if (AbilityUsesLeft <= 0)
            {
                text = "You have no abilities left.";
            }
            else
            {
                text = $"Your action target: {target.Name}.";
            }

            messages.Add(new ChatMessage("", $"TEMPLATE METHOD: {text}", user.Name, "nightNotification"));
            Console.WriteLine($"TEMPLATE METHOD: Before action of {Name}");
            return Task.CompletedTask;
        }

        // TEMPLATE METHOD: Post-action hook
        protected override Task AfterAction(Player user, Player target, List<ChatMessage> messages)
        {
            if (target != null && !target.IsAlive)
            {
                messages.Add(new ChatMessage("", $"TEMPLATE METHOD: {target.Name} has been assassinated.",
                    user.Name, "nightNotification"));
                string messageToTarget = "You were assassinated.";
                ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
                messages.Add(chatMessageToTarget);
            }
            Console.WriteLine($"TEMPLATE METHOD: After action of {Name}");
            return Task.CompletedTask;
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
