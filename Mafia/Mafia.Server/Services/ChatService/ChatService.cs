using Mafia.Server.Models;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.ChatService;

public class ChatService(IGameService gameService) : IChatService
{
    List<ChatMessage> messages = new List<ChatMessage>();
    public Task SendChatMessage(Player player, string content, string recipient, string category)
    {
        Console.WriteLine("We added a message from: $1 who has write to chat: $2" , player, content );
        messages.Add(new ChatMessage(player, content, recipient, category));
        return Task.CompletedTask;
    }
    
    

    private List<Player> GetAlivePlayers() => gameService.GetPlayers()
        .Where(x => x.IsAlive)
        .ToList();
}