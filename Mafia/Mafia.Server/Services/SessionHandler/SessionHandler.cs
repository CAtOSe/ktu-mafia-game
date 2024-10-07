using System.Net.WebSockets;
using System.Text;
using Mafia.Server.Models;
using Mafia.Server.Models.Commands;
using Mafia.Server.Services.MessageResolver;

namespace Mafia.Server.Services.SessionHandler;

public class SessionHandler(IMessageResolver messageResolver) : ISessionHandler
{

    public async Task HandleConnection(WebSocket webSocket, CancellationToken cancellationToken)
    {
        Console.WriteLine("New connection established.");
        Player player = new(webSocket);
        await player.SendMessage(new Message { Base = ResponseCommands.Hello });
        
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
            Console.WriteLine("Connection closed.");
            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                cancellationToken);
        }
    }
}
