using System.Net.WebSockets;
using Mafia.Server.Models;

namespace Mafia.Server.Services.GameService;

public interface IGameService
{
    public Task DisconnectPlayer(Player player);
    public Task TryAddPlayer(Player player, string username);
    public Task StartGame();
    public List<Player> GetPlayers();
    public Dictionary<string, string> GetPlayerRoles();
    public Task HandleMessageFromPlayer(Player player, string messageContent);
    public Task HandleIncomingMessages(WebSocket webSocket, Player player, CancellationToken stoppingToken);

    public Task NightAction(Player actionUser, string actionTarget, string actionType);
}