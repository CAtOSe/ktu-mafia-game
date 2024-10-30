using System.Net.WebSockets;
using Mafia.Server.Command;
using Mafia.Server.Models;
using Mafia.Server.Models.Messages;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.SessionHandler;
using Microsoft.AspNetCore.Mvc;

namespace Mafia.Server.Controllers
{
    [ApiController]
    [Route("api/gamecontrol")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService; 
        private readonly ISessionHandler _sessionHandler;
        private readonly IHostApplicationLifetime _hostLifetime;
        private readonly IChatService _chatService;

        public GameController(ISessionHandler sessionHandler, IHostApplicationLifetime hostLifetime, IGameService gameService, IChatService chatService)
        {
            _sessionHandler = sessionHandler;
            _hostLifetime = hostLifetime;
            _gameService = gameService;
            _chatService = chatService;
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
                    Console.WriteLine("Terminating connection.");
                }
                catch (WebSocketException e)
                {
                    if (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                    {
                        Console.WriteLine($"Unexpected connection close ({e.WebSocketErrorCode})");
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
                Console.WriteLine($"Error in TogglePauseResume: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("sendMessage")]
        public IActionResult SendMessage([FromBody] ChatMessage message)
        {
            _chatService.SendChatMessage(message); 
            return Ok();
        }
        
        [HttpPost("executeCommand")]
        public IActionResult ExecuteGameCommand([FromBody] CommandMessage command)
        {
            _gameService.ExecuteGameCommand(command);
            return Ok();
        }

    }
}
