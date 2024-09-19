using Mafia.Server.Services.SessionHandler;
using Microsoft.AspNetCore.Mvc;

namespace Mafia.Server.Controllers;

public class GameController(ISessionHandler sessionHandler) : Controller
{
    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await sessionHandler.HandleConnection(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
