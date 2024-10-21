using System;

namespace Mafia.Server.Models.Strategy
{
    public class TrackerActionPoisoned : IRoleAction
    {
        public string Name => nameof(TrackerAction);
        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            var randomPlayer = GetRandomPlayer(user, context.Players);
            string messageToUser = "";
            if (randomPlayer != null)
            {
                // Lie 1: Say they went to a random wrong player
                messageToUser = "After following the footsteps of " + target.Name + ", you find that they visited " + randomPlayer.Name + ". | FALSE";
            }
            else
            {
                // Lie 2: Say they stayed at home
                messageToUser = "You have found no footsteps of " + target.Name + ", they must have stayed at home tonight. | FALSE";
            }

            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            return Task.CompletedTask;
        }

        private Player GetRandomPlayer(Player target, List<Player> allPlayers)
        {
            Random _random = new Random();
            // Get a list of all night targets except the one we selected
            var possiblePlayers = allPlayers.Where(p => p != target).ToList();

            // Return a random player from the list
            return possiblePlayers[_random.Next(possiblePlayers.Count)];
        }
    }
}
