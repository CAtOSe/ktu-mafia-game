using Mafia.Server.Models.Iterator;

namespace Mafia.Server.Models.Strategy
{
    public class OracleActionPoisoned : IRoleAction
    {
        public string Name => nameof(OracleAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            var deadPlayersList = new DeadPlayerList(context.Players);
            var iterator = deadPlayersList.CreateIterator();

            int evilCount = 0;

            for (var deadPlayer = iterator.First(); deadPlayer != null; deadPlayer = iterator.Next()) // Iterator
            {
                if (deadPlayer.Role.Alignment == "Evil")
                {
                    evilCount++;
                }
            }

            int poisonedResult;
            if (evilCount > 0) { poisonedResult = 0; }
            else { poisonedResult = 1; }

            string messageToUser = "You sense that " + poisonedResult + " of the dead players are evil. | FALSE";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            return Task.CompletedTask;
        }
    }
}
