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
    
    public async Task<bool> IsUsernameAvailable(string username)
    {
        // Check if the username is already taken
        return !_currentPlayers.Any(player => player.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
    
    public async Task AddPlayer(Player player)
    {
         AddNewPlayer(player);
    }
    
    public void StartGame()
    {
        if (_currentPlayers.Count < 2)
        {
            throw new InvalidOperationException("There must be at least 3 players to start the game.");
        }

        // Randomly setting Killer role to 1 player
        var random = new Random();
        int killerIndex = random.Next(_currentPlayers.Count);
        Player killer = _currentPlayers[killerIndex];
        killer.Role = "Killer";
    
        // Setting Citizen role for other players
        foreach (var player in _currentPlayers)
        {
            if (player != killer)
            {
                player.Role = "Citizen";
            }

            // Siunčiame žinutę žaidėjams apie jų vaidmenį
            //player.SendMessage($"role:{player.Role}");
        }
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
