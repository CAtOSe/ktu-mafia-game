namespace Mafia.Server.Models.Strategy
{
    public class DoctorActionPoisoned : IRoleActionAlgorithm
    {
        public Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            string messageToUser = "You protected " + target.Name + " tonight. | FALSE";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);

            if (target.IsAlive) // If target was not attacked, lie and say they were
            {
                messageToUser = "Your target was attacked. | FALSE";
                chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
                nightMessages.Add(chatMessageToUser);
            }

            Console.Write(messageToUser);

            return Task.CompletedTask;
        }
    }
}
