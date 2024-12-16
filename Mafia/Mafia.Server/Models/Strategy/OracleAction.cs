using Mafia.Server.Models.Iterator;
using Mafia.Server.Models.Iterator.ActionQueue;

namespace Mafia.Server.Models.Strategy
{
    public class OracleAction : IRoleAction
    {
        public string Name => nameof(OracleAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            var deadPlayersList = new DeadPlayerList(context.Players);
            var iterator = deadPlayersList.GetIterator();

            int evilCount = 0;

            for (var deadPlayer = iterator.First(); deadPlayer != null; deadPlayer = iterator.Next()) // Iterator
            {
                if(deadPlayer.Role.Alignment == "Evil")
                {
                    evilCount++;
                }
            }

            string messageToUser = "You sense that " + evilCount + " of the dead players are evil.";
            ChatMessage chatMessageToUser = new ChatMessage("", messageToUser, user.Name, "nightNotification");
            messages.Add(chatMessageToUser);

            return Task.CompletedTask;
        }
    }
}
