using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Accomplices
{
    public class Spy : Accomplice
    {
        public Spy(int abilityUses = 10)
        {
            Name = "Spy";
            Ability = "At night, learn the role of a chosen player";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new SpyAction();
            RoleAlgorithmPoisoned = new SpyAction();
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
            if (target != null)
            {
                messages.Add(new ChatMessage("", $"TEMPLATE METHOD: You have learned that {target.Name} is a {target.Role.Name}.",
                    user.Name, "nightNotification"));
            }
            Console.WriteLine($"TEMPLATE METHOD: After action of {Name}");
            return Task.CompletedTask;
        }

        public override IRolePrototype Clone()
        {
            var clone = (Spy)this.MemberwiseClone();
            return clone;
        }
    }
}
