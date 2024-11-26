using System.Net.WebSockets;
using Mafia.Server.Command;
using Mafia.Server.Logging;
using Mafia.Server.Models;
using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.Flyweight;
using Mafia.Server.Models.Messages;
using Mafia.Server.Models.State;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.SessionHandler;
using Microsoft.AspNetCore.Mvc;
using LogLevel = Mafia.Server.Logging.LogLevel;

namespace Mafia.Server.Controllers
{
    [ApiController]
    [Route("api/gamecontrol")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService; 
        private readonly ISessionHandler _sessionHandler;
        private readonly IHostApplicationLifetime _hostLifetime;
        private readonly GameLogger _logger = GameLogger.Instance;
        private IGameState _currentState;
        //private readonly IGameStateManager _gameStateManager;
        
        public GameController(
            ISessionHandler sessionHandler,
            IHostApplicationLifetime hostLifetime,
            IGameService gameService,
            IGameStateManager gameStateManager)
        {
            _sessionHandler = sessionHandler;
            _hostLifetime = hostLifetime;
            _gameService = gameService;
            _currentState = new PlayingState(); // Primary state
            //_gameStateManager = gameStateManager;
        }

        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                try
                {
                    await _sessionHandler.HandleConnection(webSocket, _hostLifetime.ApplicationStopping);
                }
                catch (OperationCanceledException)
                {
                    _logger.Log(LogLevel.Error, "Terminating connection.");
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        _logger.Log(LogLevel.Error, $"Unexpected connection close ({e.WebSocketErrorCode})");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        // Pause/Play button logic with PauseResumeCommand
        [HttpPost("toggle")]
        public IActionResult TogglePauseResume()
        {
            try
            {
                var command = new PauseResumeCommand(_gameService, !_gameService.IsPaused, this);
                var result = command.Execute(); 

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in TogglePauseResume: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        
        //FLYWEIGHT
        [HttpGet("{roleName}")]
        public IActionResult GetRoleImage(string roleName)
        {
            // Path to the pictures folder
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "Flyweight", "pictures", $"{roleName.ToLower()}.png");

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound($"Image for role '{roleName}' not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(imagePath);
            return File(fileBytes, "image/png");
        }
        
        //STATE
        /*public void ChangeState(IGameState newState)
        {
            _currentState = newState;
            _logger.Log(LogLevel.Information, $"State changed to: {_currentState.Name}");
        }

        [HttpPost("start")]
        public IActionResult StartGame()
        {
            _currentState.Start(this);
            return Ok($"Game state is now: {_currentState.Name}");
        }

        [HttpPost("stop")]
        public IActionResult StopGame()
        {
            _currentState.Stop(this);
            return Ok($"Game state is now: {_currentState.Name}");
        }*/

        //[HttpPost("end")]
        /*public IActionResult EndGame()
        {
            _currentState.End(this);
            return Ok($"Game state is now: {_currentState.Name}");
        }*/
        /*public void EndGame()
        {
            /*ChangeState(new EndedState());
            _logger.Log(LogLevel.Information, "Game has ended.");
            _gameStateManager.EndGame();
        }*/
        public void ChangeState(IGameState newState)
        {
            _currentState = newState;
            _logger.Log(LogLevel.Information,$"State changed to: {_currentState.Name}");
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
            //Console.WriteLine("Game has ended.");
        }

    }
}
