namespace Mafia.Server.Models.Strategy
{
    public class HemlockAction : IRoleAction
    {
        public string Name => nameof(AssassinAction);

        private Player delayedVictim;

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            target.IsPoisoned = true; 

            string messageToUser = "You poisoned " + target.Name + " tonight, they will die the next night.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            if (delayedVictim != null)
            {
                delayedVictim.IsPoisoned = true;
                delayedVictim.IsAlive = false;

                messageToUser = delayedVictim.Name + " had lethal dose of poison from the previous night.";
                chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
                messages.Add(chatMessageToUser);

                string messageToTarget = "You were attacked by the Killer.";
                ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, delayedVictim.Name, "nightNotification");
                messages.Add(chatMessageToTarget);

                // Hemlock negaus zinutes kad jo praeitos nakties targetas nemire
            }

            delayedVictim = target;

            return Task.CompletedTask;
        }
    }
}
