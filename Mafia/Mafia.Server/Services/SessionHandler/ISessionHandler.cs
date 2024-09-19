using System.Net.WebSockets;

namespace Mafia.Server.Services.SessionHandler;

public interface ISessionHandler
{
    public Task HandleConnection(WebSocket webSocket);
}