using Mafia.Server.Models.Messages;

namespace Mafia.Server.Models.Bridge;

public class GameServiceHandler : IMessageHandler
{
    public void HandleChat(ChatMessage message)
    {
        
    }

    public void HandleCommand(CommandMessage message)
    {
        Console.WriteLine($"Handling game command: {message.Base}");
    }
}