using System.Text.Json;
using Mafia.Server.Models;
using Mafia.Server.Models.Commands;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.ChatService;

public class ChatService(IGameService gameService) : IChatService
{
    List<ChatMessage> messages = new List<ChatMessage>();
    public Task SendChatMessage(Player player, string content, string recipient, string category)
    {
        Console.WriteLine($"We added a message from: {player.Name} who has written to chat: {content}");
        messages.Add(new ChatMessage(player.Name, content, recipient, category));
        return SendChatList(gameService.GetPlayers()[0]);
    }

    public async Task SendChatList(Player player)
    {
        string chatMessagesJson = JsonSerializer.Serialize(messages);
        Console.WriteLine("Serialized chat messages: " + chatMessagesJson);
        await player.SendMessage(new Message
        {
            Base = ResponseCommands.ReceiveChatList,
            Arguments = new List<string> { chatMessagesJson }
        });
    } 
    

    private List<Player> GetAlivePlayers() => gameService.GetPlayers()
        .Where(x => x.IsAlive)
        .ToList();
}