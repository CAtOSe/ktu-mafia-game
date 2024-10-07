using Mafia.Server.Models;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.ChatService;

public class ChatService(IGameService gameService) : IChatService
{
    public Task HandleIncomingMessage(Player player, Message message)
    {
        return Task.CompletedTask;
    }

    private List<Player> GetAlivePlayers() => gameService.GetPlayers()
        .Where(x => x.IsAlive)
        .ToList();
}