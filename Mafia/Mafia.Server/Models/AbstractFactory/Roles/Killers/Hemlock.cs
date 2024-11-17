using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Killers
{
    public class Hemlock : Killer
    {
        public Hemlock(int abilityUses = 10)
        {
            Name = "Hemlock";
            Ability = "At night, poison a player for 2 nights, they die the next night";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new HemlockAction();
            RoleAlgorithmPoisoned = new HemlockAction();
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
            messages.Add(new ChatMessage("", $"TEMPLATE METHOD: {target.Name} has been poisoned and will die the next night.",
                user.Name, "nightNotification"));
            
            Console.WriteLine($"TEMPLATE METHOD: After action of {Name}");
            return Task.CompletedTask;
        }

        public override IRolePrototype Clone()
        {
            var clone = (Hemlock)this.MemberwiseClone();
            return clone;
        }
    }
}
