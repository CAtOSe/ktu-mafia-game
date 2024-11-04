using System.Net.WebSockets;
using System.Text;
using Mafia.Server.Extensions;
using Mafia.Server.Logging;
using Mafia.Server.Models.Messages;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.MessageResolver;

namespace Mafia.Server.Services.SessionHandler;

public class SessionHandler(IMessageResolverFacade messageResolverFacade, IGameService gameService) : ISessionHandler
{
    private GameLogger _logger = GameLogger.Instance; 
    
    public async Task HandleConnection(WebSocket webSocket, CancellationToken cancellationToken)
    {
        _logger.Log("New connection established.");
        await webSocket.SendMessage(new CommandMessage { Base = ResponseCommands.Hello }.ToString());
        
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), cancellationToken);

        while (!receiveResult.CloseStatus.HasValue)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

            await messageResolverFacade.HandleMessage(webSocket, message);

            if (webSocket.State != WebSocketState.Open && webSocket.State != WebSocketState.CloseSent)
            {
                return;
            }
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), cancellationToken);
        }

        if (webSocket.State == WebSocketState.Open)
        {
            _logger.Log("Connection closed.");
            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                cancellationToken);
        }
        
        await gameService.DisconnectPlayer(webSocket);
    }
}
