using Mafia.Server.Services.GameService;

namespace Mafia.Server.Command
{
    //Class to control Pause/Resume of the game
    public class PauseResumeCommand : ICommand
    {
        private readonly IGameService _gameService;
        private readonly bool _pause;

        public PauseResumeCommand(IGameService gameService, bool pause)
        {
            _gameService = gameService;
            _pause = pause;
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
                return "Game Paused.";
            }
            
            if (!_pause && _gameService.IsPaused)
            {
                _gameService.ResumeTimer();
                return "Game Resumed.";
            }
            
            return _gameService.IsPaused ? "Game already paused." : "Game already running.";
        }
    }
}