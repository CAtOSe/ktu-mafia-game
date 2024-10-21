namespace Mafia.Server.Models.Strategy
{
    public class DoctorAction : IRoleActionAlgorithm
    {
        public Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            string messageToUser = "You protected " + target.Name + " tonight.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            nightMessages.Add(chatMessageToUser);

            if (!target.IsAlive)
            {
                target.IsAlive = true;
                string messageToTarget = "Doctor protected you from death.";
                ChatMessage chatMessageToTarget = new ChatMessage("", messageToTarget, target.Name, "nightNotification");
                nightMessages.Add(chatMessageToTarget);

                messageToUser = "Your target was attacked.";
                chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
                nightMessages.Add(chatMessageToUser);

                string killer = nightActions.FirstOrDefault(p => p.ActionType == "kill" && p.Target == target).User.Name;
                string messageToKiller = "Your target survived the attack.";
                ChatMessage chatMessageToKiller = new ChatMessage("", messageToKiller, killer, "nightNotification");
                nightMessages.Add(chatMessageToKiller);
            }

            Console.Write(messageToUser);

            return Task.CompletedTask;
        }
    }
}
