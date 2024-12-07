using Mafia.Server.Models.State;
using Mafia.Server.Services.GameService;
using Mafia.Server.Models.Memento;

namespace Mafia.Server.Command
{
    public class PauseResumeCommand : ICommand
    {
        private readonly IGameService _gameService;
        private readonly bool _pause;
        private readonly IGameStateManager _gameStateManager;
        private readonly GameStateCaretaker _gameStateCaretaker;

        public PauseResumeCommand(IGameService gameService, bool pause, 
            IGameStateManager gameStateManager, GameStateCaretaker gameStateCaretaker)
        {
            _gameService = gameService;
            _pause = pause;
            _gameStateManager = gameStateManager;
            _gameStateCaretaker = gameStateCaretaker;
        }

        public string Execute()
        {
            if (!_gameService.GameStarted)
            {
                return "Game not started.";
            }

            if (_pause && !_gameService.IsPaused)
            {
                //MEMENTO
                var memento = new GameStateMemento(_gameService.GetPlayers(), _gameService.GameStarted, _gameService.IsPaused);
                _gameStateCaretaker.SaveState(memento);
                
                _gameService.PauseTimer();
                _gameStateManager.StopGame();
                return "Game Paused.";
            }

            if (!_pause && _gameService.IsPaused)
            {
                //MEMENTO
                var memento = _gameStateCaretaker.RestoreState();
                if (memento != null)
                {
                    _gameService.RestoreGameState(memento);
                }
                
                _gameService.ResumeTimer();
                _gameStateManager.StartGame();
                return "Game Resumed.";
            }

            return _gameService.IsPaused ? "Game already paused." : "Game already running.";
        }
    }
}