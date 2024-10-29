using Mafia.Server.Models.Messages;

namespace Mafia.Server.Models.Bridge;

public interface IMessageHandler
{
    void HandleChat(ChatMessage message);
    void HandleCommand(CommandMessage message);
}
