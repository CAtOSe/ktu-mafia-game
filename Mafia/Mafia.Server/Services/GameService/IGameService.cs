using Mafia.Server.Models;

namespace Mafia.Server.Services.GameService;

public interface IGameService
{
    public void AddNewPlayer(Player player);
    public void RemovePlayer(Player player); 
    Task<bool> IsUsernameAvailable(string username);
    Task AddPlayer(Player player);
    public void StartGame();
    public void NotifyAllPlayers(Player player, string action);
    public List<Player> GetPlayers();
}