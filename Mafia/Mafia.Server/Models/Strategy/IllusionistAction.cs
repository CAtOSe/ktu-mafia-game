using Mafia.Server.Models.Iterator;

namespace Mafia.Server.Models.Strategy
{
    public class IllusionistAction : IRoleAction
    {
        public string Name => nameof(IllusionistAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            user.IsAlive = false;

            var goodPlayersList = new GoodPlayerList(context.Players);
            var iterator = goodPlayersList.GetIterator();

            for (var goodPlayer = iterator.First(); goodPlayer != null; goodPlayer = iterator.Next()) // Iterator
            {
                goodPlayer.IsPoisoned = true;
            }

            const string messageToUser = "Your grand performance poisoned every good player.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);
            return Task.CompletedTask;
        }
    }
}
