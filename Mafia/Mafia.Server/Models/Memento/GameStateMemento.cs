namespace Mafia.Server.Models.Memento
{
    public class GameStateMemento
    {
        public List<Player> Players { get; private set; }
        public bool GameStarted { get; private set; }
        public bool IsPaused { get; private set; }

        public GameStateMemento(List<Player> players, bool gameStarted, bool isPaused)
        {
            // Copying PLayer data
            Players = players.Select(player => new Player(player.WebSocket)
            {
                Name = player.Name,
                Role = player.Role,
                IsLoggedIn = player.IsLoggedIn,
                IsHost = player.IsHost,
                IsAlive = player.IsAlive,
                IsPoisoned = player.IsPoisoned
            }).ToList();
            
            GameStarted = gameStarted;
            IsPaused = isPaused;
        }
    }
}