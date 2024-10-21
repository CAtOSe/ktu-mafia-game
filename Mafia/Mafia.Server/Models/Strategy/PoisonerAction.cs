namespace Mafia.Server.Models.Strategy
{
    public class PoisonerAction : IRoleAction
    {
        public string Name => nameof(PoisonerAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            string messageToUser = "You poisoned " + target.Name + " tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            target.IsPoisoned = true;

            Console.Write(messageToUser);

            return Task.CompletedTask;
        }
    }
}
