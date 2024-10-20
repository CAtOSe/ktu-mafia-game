using System;

namespace Mafia.Server.Models.Strategy
{
    public class TrackerActionPoisoned : IRoleActionAlgorithm
    {
        public Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            var action = nightActions.FirstOrDefault(p => p.User.Name.Equals(target.Name, StringComparison.OrdinalIgnoreCase));

            string wentTo = action?.Target?.Name;

            var randomPlayer = GetRandomPlayer(user, nightActions);
            string messageToUser = "";
            if ((wentTo == target.Name || wentTo == null) && randomPlayer != null)
            {
                // Lie 1: Say they went to a random wrong player
                messageToUser = "After following the footsteps of " + target.Name + ", you find that they visited " + user.Name + ". | FALSE";
            }
            else
            {
                // Lie 2: Say they stayed at home
                messageToUser = "You have found no footsteps of " + target.Name + ", they must have stayed at home tonight. | FALSE";
            }

            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);

            return Task.CompletedTask;
        }

        private Player GetRandomPlayer(Player target, List<NightAction> nightActions)
        {
            Random _random = new Random();
            // Get a list of all night targets except the one we selected
            var possiblePlayers = nightActions
                .Select(na => na.Target)
                .Where(p => p.Name != target.Name)
                .ToList();

            // Return a random player from the list
            return possiblePlayers[_random.Next(possiblePlayers.Count)];
        }
    }
}
