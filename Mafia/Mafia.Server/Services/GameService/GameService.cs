using Mafia.Server.Models;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly List<Player> _currentPlayers = [];

    public void AddNewPlayer(Player player)
    {
        _currentPlayers.Add(player);
        player.SendMessage(Messages.LoggedIn);
        NotifyAllPlayers(player, "new-player");
    }

    public void RemovePlayer(Player player)
    {
        _currentPlayers.Remove(player);
        player.CloseConnection();
    }
    
    private void NotifyAllPlayers(Player newPlayer, string action)
    {
        foreach (var player in _currentPlayers)
        {
            if (player != newPlayer)
            {
                player.SendMessage($"{action}:{newPlayer.Name}"); // Pvz.: "new-player:Player1"
            }
        }
    }
}
