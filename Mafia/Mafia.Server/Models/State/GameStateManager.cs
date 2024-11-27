using Mafia.Server.Logging;
using LogLevel = Mafia.Server.Logging.LogLevel;

namespace Mafia.Server.Models.State
{
    public class GameStateManager : IGameStateManager
    {
        private IGameState _currentState;
        private readonly GameLogger _logger = GameLogger.Instance;

        public GameStateManager()
        {
<<<<<<< Updated upstream
            _currentState = new StoppedState(); // Primary state
=======
            _currentState = new PlayingState(); // Primary state
>>>>>>> Stashed changes
        }

        public void ChangeState(IGameState newState)
        {
            _currentState = newState;
            _logger.Log(LogLevel.Information, $"State changed to: {_currentState.Name}");
        }

        public void StartGame()
        {
            ChangeState(new PlayingState());
        }

        public void StopGame()
        {
            ChangeState(new StoppedState());
        }

        public void EndGame()
        {
            ChangeState(new EndedState());
        }
    }
}