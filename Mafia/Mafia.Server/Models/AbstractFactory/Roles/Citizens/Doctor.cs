using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;
using System.Security.Cryptography.X509Certificates;

namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Doctor : Citizen
    {
        public Doctor(int abilityUses = 10)
        {
            Name = "Doctor";
            Ability = "At night, choose a player to protect them from dying";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new DoctorAction();
            RoleAlgorithmPoisoned = new DoctorActionPoisoned();
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
                target.IsAlive = true;
                if (AbilityUsesLeft > 0) AbilityUsesLeft--; // Decrease ability uses
                messages.Add(new ChatMessage("", $"TEMPLATE METHOD: Action executed on: {target.Name}!",
                    target.Name, "nightNotification"));
            }
            Console.WriteLine($"TEMPLATE METHOD: After action of {Name}");
            return Task.CompletedTask;
        }

        public override IRolePrototype Clone()
        {
            var clone = (Doctor)this.MemberwiseClone();
            return clone;
        }
    }
}
