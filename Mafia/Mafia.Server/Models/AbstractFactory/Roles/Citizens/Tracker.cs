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
