using Mafia.Server.Models.Strategy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Mafia.Server.Models.Iterator;
public class ActionQueue : IActionQueue // Concrete Aggregator
{
    private readonly List<ActionQueueEntry> _entries;

    // Custom order for sorting actions
    private readonly List<string> _actionOrder = new()
    {
        "Poisoner", "Assassin", "Hemlock",
        "Soldier", "Tracker", "Lookout", "Doctor"
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