using System.Net.WebSockets;
using Moq;

namespace Mafia.Server.Tests.Unit.TestSetup;

public static class MockWebSocket
{
    public static WebSocket Get()
    {
        return new Mock<WebSocket>().Object;
    }
}