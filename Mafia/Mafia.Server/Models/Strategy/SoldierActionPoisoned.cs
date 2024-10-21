namespace Mafia.Server.Models.Strategy
{
    public class SoldierActionPoisoned : IRoleActionAlgorithm
    {
        public Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            string messageToUser = "You used your shield to protect yourself tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);

            if (target.IsAlive) // If you were not attacked, say you were
            {
                string messageToTarget = "Your shield protected you from death. | FALSE";
                ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
                nightMessages.Add(chatMessageToTarget);
            }
            else if (!target.IsAlive) {
                string messageToTarget = "Unexpectedly, your shield did not protect you from death.";
                ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
                nightMessages.Add(chatMessageToTarget);
            }

            return Task.CompletedTask;
        }
    }
}
