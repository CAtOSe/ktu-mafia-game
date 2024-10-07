using Mafia.Server.Models;

namespace Mafia.Server.Services.ChatService;

public interface IChatService
{
    public Task HandleIncomingMessage(Player player, Message message);
}