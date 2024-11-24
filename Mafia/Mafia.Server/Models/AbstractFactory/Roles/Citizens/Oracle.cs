using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Oracle : Citizen
    {
        public Oracle(int abilityUses = 10)
        {
            Name = "Oracle";
            Ability = "At night, learn how many dead players are evil";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new OracleAction();
            RoleAlgorithmPoisoned = new OracleActionPoisoned();
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
            messages.Add(new ChatMessage("", $"TEMPLATE METHOD: Action executed on: {target.Name}!",
                target.Name, "nightNotification"));

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
            var clone = (Oracle)this.MemberwiseClone();
            return clone;
        }
    }
}
