using System.Net.WebSockets;
using Mafia.Server.Command;
using Mafia.Server.Logging;
using Mafia.Server.Models.State;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.SessionHandler;
using Microsoft.AspNetCore.Mvc;
using LogLevel = Mafia.Server.Logging.LogLevel;
using Mafia.Server.Models.Memento;

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
        private readonly IGameStateManager _gameStateManager;
        private readonly GameStateCaretaker _gameStateCaretaker;

        public GameController(
            ISessionHandler sessionHandler,
            IHostApplicationLifetime hostLifetime,
            IGameService gameService,
            IGameStateManager gameStateManager)
        {
            _sessionHandler = sessionHandler;
            _hostLifetime = hostLifetime;
            _gameService = gameService;
            _gameStateManager = gameStateManager;
            _gameStateCaretaker = new GameStateCaretaker();
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
                var command = new PauseResumeCommand(_gameService, !_gameService.IsPaused, _gameStateManager, _gameStateCaretaker);
                var result = command.Execute();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in TogglePauseResume: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        
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

        [HttpPost("start")]
        public IActionResult StartGame()
        {
            _gameStateManager.StartGame();
            return Ok("Game started.");
        }

        [HttpPost("stop")]
        public IActionResult StopGame()
        {
            _gameStateManager.StopGame();
            return Ok("Game stopped.");
        }

        [HttpPost("end")]
        public IActionResult EndGame()
        {
            _gameStateManager.EndGame();
            return Ok("Game ended.");
        }
    }
}
