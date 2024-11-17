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
            if (target != null)
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
            messages.Add(new ChatMessage("", $"TEMPLATE METHOD: You have learned that {target.Name} is a {target.Role.Name}.",
                user.Name, "nightNotification"));
            
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
