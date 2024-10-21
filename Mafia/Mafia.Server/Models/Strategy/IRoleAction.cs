namespace Mafia.Server.Models.Strategy
{
    public interface IRoleAction
    {
        public string Name { get; }
        public Task Execute(Player user, Player target, RoleActionContext context, List<ChatMessage> messages);
    }
}
