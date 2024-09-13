using Mafia.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Mafia.Server.Controllers;

public class GameController : Controller
{
    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await GameService.HandleGameSocket(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
