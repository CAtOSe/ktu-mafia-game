using Mafia.Server.Models;

namespace Mafia.Server.Services.GameService;

public interface IGameService
{
    public Task DisconnectPlayer(Player player); 
    public Task TryAddPlayer(Player player, string username);
    public void AddNewPlayer(Player player);
    public void RemovePlayer(Player player); 
    Task<bool> IsUsernameAvailable(string username);
    Task AddPlayer(Player player);
    public void StartGame();
    public void NotifyAllPlayers(Player player, string action);
    public List<Player> GetPlayers();
    public Dictionary<string, string> GetPlayerRoles();
}