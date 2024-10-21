using Mafia.Server.Models;

namespace Mafia.Server.Services.ChatService;

public interface IChatService
{
    public Task SendChatMessage(string sender, string content, string recipient, string category);
    public Task SendChatMessage(ChatMessage chatMessage);
    public void SetPlayers(List<Player> newPlayers);
    void ResetChat();
}