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
            messages.Add(new ChatMessage("", $"You have tracked {target.Name} and found their activities.",
                user.Name, "nightNotification"));
            
            Console.WriteLine($"TEMPLATE METHOD: After action of {Name}");
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
