namespace Mafia.Server.Models.Strategy;

public record ActionQueueEntry
{
    public Player User { get; set; }
    public Player Target { get; set; }
}