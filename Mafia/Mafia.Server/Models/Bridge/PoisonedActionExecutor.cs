using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.Bridge;

public class PoisonedActionExecutor : IRoleActionExecutor
{
    private readonly IRoleAction _poisonedAction;

    public PoisonedActionExecutor(IRoleAction poisonedAction)
    {
        _poisonedAction = poisonedAction;
    }

    public Task ExecuteAction(Player user, Player target, RoleActionContext context, List<ChatMessage> messages)
    {
        _poisonedAction.Execute(user, target, context, messages);
        return Task.CompletedTask;
    }
}