using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Tracker : Citizen
    {
        public Tracker(int abilityUses = 10)
        {
            Name = "Tracker";
            Ability = "At night, choose a player to find out who they visited that night";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new TrackerAction();
            RoleAlgorithmPoisoned = new TrackerActionPoisoned();
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
            if (AbilityUsesLeft > 0) AbilityUsesLeft--; // Decrease ability uses
            messages.Add(new ChatMessage("", $"You have tracked {target.Name} and found their activities.",
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
            var clone = (Tracker)this.MemberwiseClone();
            return clone;
        }

        /*
        public override void NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            var action = nightActions.FirstOrDefault(p => p.User.Name.Equals(target.Name, StringComparison.OrdinalIgnoreCase));

            string wentTo = action?.Target?.Name;

            string messageToUser = ""; 
            if (wentTo == target.Name || wentTo == null)
            {
                messageToUser = "You have found no foosteps of " + target.Name + ", they must have stayed at home tonight.";
            }
            else
            {
               messageToUser = "After following the footsteps of " + target.Name + ", you find that that they visited " + wentTo + ".";
            }
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);
        }*/
    }
}
