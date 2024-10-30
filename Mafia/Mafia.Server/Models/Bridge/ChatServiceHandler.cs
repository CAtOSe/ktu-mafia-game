using Mafia.Server.Models.Messages;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Models.Bridge;

public class ChatServiceHandler : IMessageHandler
{
    public void HandleChat(ChatMessage message)
    {
        Console.WriteLine($"Handling chat message: {message.Content}");
    }

    public void HandleCommand(CommandMessage message)
    {
       
    }
}