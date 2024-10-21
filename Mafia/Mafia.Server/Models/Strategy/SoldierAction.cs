namespace Mafia.Server.Models.Strategy
{
    public class SoldierAction : IRoleAction
    {
        public string Name => nameof(SoldierAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            string messageToUser = "You used your shield to protect yourself tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            if (!target.IsAlive)
            {
                target.IsAlive = true;
                string messageToTarget = "Your shield protected you from death.";
                ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
                messages.Add(chatMessageToTarget);
            }

            return Task.CompletedTask;
        }
    }
}
