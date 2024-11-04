namespace Mafia.Server.Models.Strategy
{
    public class DoctorAction : IRoleAction
    {
        public string Name => nameof(DoctorAction);
        
        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            string messageToUser = "You protected " + target.Name + " tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            if (!target.IsAlive)
            {
                target.IsAlive = true;
                string messageToTarget = "Doctor protected you from death.";
                ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
                messages.Add(chatMessageToTarget);

                messageToUser = "Your target was attacked.";
                chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
                messages.Add(chatMessageToUser);

                var killer = context.QueuedActions.FirstOrDefault(a => a.Target == target && a.User != user)?.Target;
                if (killer is not null)
                {
                    string messageToKiller = "Your target survived the attack.";
                    ChatMessage chatMessageToKiller = new ChatMessage("", messageToKiller, killer.Name, "nightNotification");
                    messages.Add(chatMessageToKiller);
                }
            }

            return Task.CompletedTask;
        }
    }
}
