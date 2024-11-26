using Mafia.Server.Controllers;
using Mafia.Server.Models.State;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Command
{
    //Class to control Pause/Resume of the game
    public class PauseResumeCommand : ICommand
    {
        private readonly IGameService _gameService;
        private readonly bool _pause;
        private GameController _gameController;
        //private readonly IGameStateManager _gameStateManager;

        public PauseResumeCommand(IGameService gameService, bool pause, /*IGameStateManager gameStateManager*/ GameController gameController)
        {
            _gameService = gameService;
            _pause = pause;
            _gameController = gameController;
            //_gameStateManager = gameStateManager;
        }

        public string Execute()
        {
            if (_gameService == null)
            {
                return "GameService is null.";
            }

            if (_gameController == null)
            {
                return "GameStateManager is null.";
            }
            
            if (!_gameService.GameStarted)
            {
                return "Game not started.";
            }
            
            if (_pause && !_gameService.IsPaused)
            {
                _gameService.PauseTimer();
                _gameController.StopGame(); // STATE
                return "Game Paused.";
            }
            
            if (!_pause && _gameService.IsPaused)
            {
                _gameService.ResumeTimer();
                _gameController.StartGame(); // STATE
                return "Game Resumed.";
            }

            return _gameService.IsPaused ? "Game already paused." : "Game already running.";
        }
    }
}