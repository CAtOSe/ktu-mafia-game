namespace Mafia.Server.Models.Strategy
{
    public class PoisonerAction : IRoleActionAlgorithm
    {
        public Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            string messageToUser = "You poisoned " + target.Name + " tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);

            target.IsPoisoned = true;

            Console.Write(messageToUser);

            return Task.CompletedTask;
        }
    }
}
