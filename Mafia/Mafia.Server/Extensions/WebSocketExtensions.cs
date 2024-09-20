using System.Net.WebSockets;
using System.Text;

namespace Mafia.Server.Extensions;

public static class WebSocketExtensions
{
    public static ValueTask SendMessage(this WebSocket socket, string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        return socket.SendAsync(buffer,
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }
}