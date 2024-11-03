using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.Bridge;
public class StandardActionExecutor : IRoleActionExecutor
{
    private readonly IRoleAction _roleAction;

    public StandardActionExecutor(IRoleAction roleAction)
    {
        _roleAction = roleAction;
    }

    public Task ExecuteAction(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
    {
        _roleAction.Execute(user, target, context, messages);
        return Task.CompletedTask;
    }
}