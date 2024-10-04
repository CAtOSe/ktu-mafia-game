using System.Net.WebSockets;
using System.Text;
using Mafia.Server.Models;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.MessageResolver;

namespace Mafia.Server.Services.SessionHandler;

public class SessionHandler(
    IMessageResolver messageResolver,
    IGameService gameService
    ) : ISessionHandler
{

    public async Task HandleConnection(WebSocket webSocket, CancellationToken cancellationToken)
    {
        Player player = new(webSocket);
        
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), cancellationToken);

        while (!receiveResult.CloseStatus.HasValue)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

            await messageResolver.HandleMessage(player, message);
            
            if (webSocket.State != WebSocketState.Open && webSocket.State != WebSocketState.CloseSent) return;
            receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), cancellationToken);
        }

        if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                cancellationToken);
        }
        await gameService.DisconnectPlayer(player);
    }
}
