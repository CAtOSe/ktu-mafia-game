namespace Mafia.Server.Models.Strategy;

public class RoleActionContext
{
    public List<Player> Players { get; set; }
    public List<ActionQueueEntry> QueuedActions { get; set; }
}