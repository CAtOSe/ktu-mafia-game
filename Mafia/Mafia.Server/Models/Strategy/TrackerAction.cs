namespace Mafia.Server.Models.Strategy
{
    public class TrackerAction : IRoleAction
    {
        public string Name => nameof(TrackerAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            var action = context.QueuedActions.FirstOrDefault(p => p.User.Name.Equals(target.Name, StringComparison.OrdinalIgnoreCase));

            string wentTo = action?.Target?.Name;

            string messageToUser = "";
            if (wentTo == null)
            {
                messageToUser = "You have found no foosteps of " + target.Name + ", they must have stayed at home tonight.";
            }
            else
            {
                messageToUser = "After following the footsteps of " + target.Name + ", you find that that they visited " + wentTo + ".";
            }
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            return Task.CompletedTask;
        }
    }
}
