using Mafia.Server.Models.State;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Command
{
    public class PauseResumeCommand : ICommand
    {
        private readonly IGameService _gameService;
        private readonly bool _pause;
        private readonly IGameStateManager _gameStateManager;

        public PauseResumeCommand(IGameService gameService, bool pause, IGameStateManager gameStateManager)
        {
            _gameService = gameService;
            _pause = pause;
            _gameStateManager = gameStateManager;
        }

        public string Execute()
        {
            if (!_gameService.GameStarted)
            {
                return "Game not started.";
            }

            if (_pause && !_gameService.IsPaused)
            {
                _gameService.PauseTimer();
                _gameStateManager.StopGame();
                return "Game Paused.";
            }

            if (!_pause && _gameService.IsPaused)
            {
                _gameService.ResumeTimer();
                _gameStateManager.StartGame();
                return "Game Resumed.";
            }

            return _gameService.IsPaused ? "Game already paused." : "Game already running.";
        }
    }
}