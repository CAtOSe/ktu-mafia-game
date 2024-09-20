using Mafia.Server.Models;

namespace Mafia.Server.Services.GameService;

public interface IGameService
{
    public void AddNewPlayer(Player player);
    public void RemovePlayer(Player player);
}