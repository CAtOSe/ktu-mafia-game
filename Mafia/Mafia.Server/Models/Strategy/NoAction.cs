namespace Mafia.Server.Models.Strategy
{
    public class NoAction : IRoleAction
    {
        public string Name => nameof(NoAction);

        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
        {
            // Does nothing
            return Task.CompletedTask;
        }
    }
}
