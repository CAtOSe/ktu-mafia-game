namespace Mafia.Server.Models.Strategy
{
    public class AssassinAction : IRoleActionAlgorithm
    {
        public Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            target.IsAlive = false;

            string messageToUser = "You attacked " + target.Name + " tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);

            string messageToTarget = "You were attacked by the Killer."; // Death Message
            ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
            nightMessages.Add(chatMessageToTarget);

            return Task.CompletedTask;
        }
    }
}
