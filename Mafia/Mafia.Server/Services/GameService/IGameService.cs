using System.Net.WebSockets;
using Mafia.Server.Models;
using Mafia.Server.Models.Memento;
using Mafia.Server.Models.Messages;

namespace Mafia.Server.Services.GameService;

public interface IGameService
{
    public Task DisconnectPlayer(WebSocket webSocket);
    public Task TryAddPlayer(WebSocket webSocket, string username);
    public Task StartGame(string difficultyLevel);
    public List<Player> GetPlayers();
    public Task AddNightActionToList(Player actionUser, string actionTarget, string actionType);
    public Task VoteFor(Player player, string username);
    void PauseTimer();
    void ResumeTimer();
    public bool IsPaused { get; }
    public bool GameStarted { get; }
    public void RestoreGameState(GameStateMemento memento);
}