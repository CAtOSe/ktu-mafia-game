using System.Net.WebSockets;
using Mafia.Server.Models;

namespace Mafia.Server.Services.GameService;

public interface IGameService
{
    public Task DisconnectPlayer(Player player);
    public Task TryAddPlayer(Player player, string username);
    public Task StartGame(string difficultyLevel);
    public List<Player> GetPlayers();
    public Task AddNightActionToList(Player actionUser, string actionTarget, string actionType);
    public Task VoteFor(Player player, string username);
    void PauseTimer();
    void ResumeTimer();
    public bool IsPaused { get; }
    public bool GameStarted { get; }
}