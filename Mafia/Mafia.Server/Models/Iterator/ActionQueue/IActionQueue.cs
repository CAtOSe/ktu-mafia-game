namespace Mafia.Server.Models.Iterator.ActionQueue
{
    public interface IActionQueue // Aggregator
    {
        IActionQueueIterator CreateIterator();
    }
}
