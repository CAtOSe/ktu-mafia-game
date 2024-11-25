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
        //private IGameState _currentState;
        private GameController _gameController;

        public PauseResumeCommand(IGameService gameService, bool pause, GameController gameController)
        {
            _gameService = gameService;
            _pause = pause;
            _gameController = gameController;
            //_currentState = new StoppedState();
        }

        public string Execute()
        {
            if (_gameService == null)
            {
                return "GameService is null.";
            }

            if (_gameController == null)
            {
                return "GameController is null.";
            }
            
            if (!_gameService.GameStarted)
            {
                return "Game not started.";
            }
            
            if (_pause && !_gameService.IsPaused)
            {
                _gameController.StopGame();
                _gameService.PauseTimer();
                
                return "Game Paused.";
            }
            
            if (!_pause && _gameService.IsPaused)
            {
                _gameService.ResumeTimer();
                _gameController.StartGame();
                return "Game Resumed.";
            }
            //_gameController.StartGame();
            return _gameService.IsPaused ? "Game already paused." : "Game already running.";
        }
    }
}