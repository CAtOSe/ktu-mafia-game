using Mafia.Server.Models.Strategy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Mafia.Server.Models.Iterator.ActionQueue;
public class ActionQueue : IActionQueue // Concrete Aggregator
{
    private readonly List<ActionQueueEntry> _entries;

    // Custom order for sorting actions
    private readonly List<string> _actionOrder = new()
    {
        "Illusionist", "Poisoner", "Assassin", "Hemlock",
        "Soldier", "Doctor", "Tracker", "Lookout", "Oracle"
    };

    public ActionQueue(IEnumerable<ActionQueueEntry> entries)
    {
        _entries = entries.ToList();
    }

    public IActionQueueIterator CreateIterator()
    {
        return new ActionQueueIterator(_entries, _actionOrder);
    }
}