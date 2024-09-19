using Mafia.Server.Models;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly List<Player> _currentPlayers = [];

    public void AddNewPlayer(Player player)
    {
        _currentPlayers.Add(player);
        player.SendMessage(Messages.LoggedIn);
    }

    public void RemovePlayer(Player player)
    {
        _currentPlayers.Remove(player);
        player.CloseConnection();
    }
}
