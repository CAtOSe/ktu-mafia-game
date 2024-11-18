using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.Iterator
{
    public interface IActionQueueIterator
    {
        ActionQueueEntry First();
        ActionQueueEntry Next();
        bool HasMore();
        ActionQueueEntry Current { get; }
    }

}
