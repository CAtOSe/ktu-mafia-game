namespace Mafia.Server.Models.Iterator
{
    public interface IActionQueue // Aggregator
    {
        IActionQueueIterator CreateIterator();
    }
}
