namespace Mafia.Server.Models.Iterator.ActionQueue
{
    using Mafia.Server.Models.Strategy;
    using System.Collections.Generic;
    using System.Linq;

    public class ActionQueueIterator : IActionQueueIterator // Concrete Iterator
    {
        private readonly List<ActionQueueEntry> _entries;
        private int _currentIndex = -1;

        public ActionQueueIterator(IEnumerable<ActionQueueEntry> entries, List<string> actionOrder)
        {
            // Sort entries based on the custom action order
            _entries = entries
                .OrderBy(entry => actionOrder.IndexOf(entry.User.RoleName))
                .ToList();
        }

        public ActionQueueEntry First()
        {
            _currentIndex = 0;
            return _entries.Count > 0 ? _entries[_currentIndex] : null;
        }

        public ActionQueueEntry Next()
        {
            _currentIndex++;
            return HasMore() ? _entries[_currentIndex] : null;
        }

        public bool HasMore()
        {
            return _currentIndex < _entries.Count - 1;
        }

        public ActionQueueEntry Current => _currentIndex >= 0 && _currentIndex < _entries.Count ? _entries[_currentIndex] : null;
    }

}
