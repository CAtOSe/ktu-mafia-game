namespace Mafia.Server.Models.Strategy
{
    public class SpyAction : IRoleAction
    {
        public string Name => nameof(PoisonerAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            string messageToUser = "You learn that " + target.Name + " is the " + target.RoleName + ".";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            Console.Write(messageToUser);

            return Task.CompletedTask;
        }
    }
}
