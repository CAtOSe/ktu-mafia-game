using System.Net.WebSockets;
using Mafia.Server.Models;

namespace Mafia.Server.Services.MessageResolver;

public interface IMessageResolverFacade
{
    public Task HandleMessage(WebSocket webSocket, string message);
}