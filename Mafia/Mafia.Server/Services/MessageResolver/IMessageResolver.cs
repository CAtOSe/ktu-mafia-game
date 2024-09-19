using Mafia.Server.Models;

namespace Mafia.Server.Services.MessageResolver;

public interface IMessageResolver
{
    public Task HandleMessage(Player player, string message);
}