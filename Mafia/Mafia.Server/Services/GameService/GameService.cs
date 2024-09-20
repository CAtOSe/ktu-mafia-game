using Mafia.Server.Models;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly List<Player> _currentPlayers = [];

    public void AddNewPlayer(Player player)
    {
        /*_currentPlayers.Add(player); // OLD one
        player.SendMessage(Messages.LoggedIn);
        NotifyAllPlayers(player, "new-player");*/
        
        // Adding new player to list
        _currentPlayers.Add(player);

        // Sending message to new player with all player list
        var allPlayers = string.Join(",", _currentPlayers.Select(p => p.Name));
        player.SendMessage($"players-list:{allPlayers}");

        // Notifying all other players about new player
        NotifyAllPlayers(player, "new-player");

        // Inform new player, that he successfully logged in
        player.SendMessage(Messages.LoggedIn);
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
                player.SendMessage($"{action}:{newPlayer.Name}"); 
            }
        }
    }
}
