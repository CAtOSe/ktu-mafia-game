using System.Net.WebSockets;
using Mafia.Server.Command;
using Mafia.Server.Logging;
using Mafia.Server.Models;
using Mafia.Server.Models.Messages;
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

        public GameController(
            ISessionHandler sessionHandler,
            IHostApplicationLifetime hostLifetime,
            IGameService gameService)
        {
            _sessionHandler = sessionHandler;
            _hostLifetime = hostLifetime;
            _gameService = gameService;
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
                var command = new PauseResumeCommand(_gameService, !_gameService.IsPaused);
                var result = command.Execute(); 

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error in TogglePauseResume: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
