using Mafia.Server.Services.SessionHandler;
using Microsoft.AspNetCore.Mvc;

namespace Mafia.Server.Controllers;

public class GameController(ISessionHandler sessionHandler, IHostApplicationLifetime hostLifetime) : Controller
{
    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            try
            {
                await sessionHandler.HandleConnection(webSocket, hostLifetime.ApplicationStopping);
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"Terminating connection.");
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
