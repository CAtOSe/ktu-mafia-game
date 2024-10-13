using Mafia.Server.Models;

namespace Mafia.Server.Services.ChatService;

public interface IChatService
{
    public Task SendChatMessage(Player player, string content, string recipient, string category);
}