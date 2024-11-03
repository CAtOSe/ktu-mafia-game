using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.Bridge
{
    public interface IRoleActionExecutor
    {
        Task ExecuteAction(Player user, Player target, RoleActionContext context, List<ChatMessage> messages);
    }
}