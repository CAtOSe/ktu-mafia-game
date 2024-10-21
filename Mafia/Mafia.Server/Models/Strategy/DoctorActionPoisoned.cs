namespace Mafia.Server.Models.Strategy
{
    public class DoctorActionPoisoned : IRoleAction
    {
        public string Name => nameof(DoctorAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            string messageToUser = "You protected " + target.Name + " tonight. | FALSE";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            if (target.IsAlive) // If target was not attacked, lie and say they were
            {
                messageToUser = "Your target was attacked. | FALSE";
                chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
                messages.Add(chatMessageToUser);
            }

            Console.Write(messageToUser);

            return Task.CompletedTask;
        }
    }
}
