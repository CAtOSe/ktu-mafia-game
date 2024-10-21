namespace Mafia.Server.Models.Strategy
{
    public class AssassinAction : IRoleAction
    {
        public string Name => nameof(AssassinAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            target.IsAlive = false;

            string messageToUser = "You attacked " + target.Name + " tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            string messageToTarget = "You were attacked by the Killer."; // Death Message
            ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
            messages.Add(chatMessageToTarget);

            return Task.CompletedTask;
        }
    }
}
