using System.Net.WebSockets;
using Mafia.Server.Models.Messages;
using Moq;

namespace Mafia.Server.Tests.Unit.TestSetup;

public class MockSocketHandler
{
    private List<string> _expectedBaseMessages = new();
    private List<string> _expectedFullMessages = new();
    private List<string> _expectedCommands = new();

    private List<string> _capturedMessages = new();
    
    public WebSocket Socket { get; private set; }

    public MockSocketHandler()
    {
        Socket = new TestWebSocket(_capturedMessages);
    }

    public static WebSocket Get()
    {
        return new Mock<WebSocket>().Object;
    }

    public void SetupExpectedMessage(CommandMessage message)
    {
        _expectedCommands.Add(message.ToString());
    }
    
    public void SetupExpectedMessage(string message)
    {
        _expectedFullMessages.Add(message);
    }

    public void SetupExpectedBase(string baseMessage)
    {
        _expectedBaseMessages.Add(baseMessage);
    }

    public bool Verify()
    {
        foreach (var baseMessage in _expectedBaseMessages)
        {
            if (!_capturedMessages.Any(x => x.StartsWith($"{baseMessage}:")))
            {
                throw new Exception($"Did not capture expected base message '{baseMessage}' in test web socket.");
            }
        }
        
        foreach (var message in _expectedFullMessages)
        {
            if (!_capturedMessages.Any(x => x.Equals(message)))
            {
                throw new Exception($"Did not capture expected message '{message}' in test web socket.");
            }
        }
        
        foreach (var message in _expectedCommands)
        {
            if (!_capturedMessages.Any(x => x.Equals(message)))
            {
                throw new Exception($"Did not capture command '{message}' in test web socket.");
            }
        }

        return true;
    }
}