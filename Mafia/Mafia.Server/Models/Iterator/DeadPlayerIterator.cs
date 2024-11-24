namespace Mafia.Server.Models.Iterator
{
    using Mafia.Server.Models.Strategy;
    using System.Collections.Generic;
    using System.Linq;

    public class DeadPlayerIterator : IPlayerIterator // Concrete Iterator
    {
        private readonly List<Player> _entries;
        private int _currentIndex = -1;

        public DeadPlayerIterator(IEnumerable<Player> entries)
        {
            // Filter dead players
            _entries = entries.Where(player => !player.IsAlive).ToList();

        }

        public Player First()
        {
            _currentIndex = 0;
            return _entries.Count > 0 ? _entries[_currentIndex] : null;
        }

        public Player Next()
        {
            _currentIndex++;
            return IsDone() ? null : _entries[_currentIndex];
        }

        public bool IsDone()
        {
            return _currentIndex >= _entries.Count;
        }

        public Player Current()
        {
            if (_currentIndex >= 0 && _currentIndex < _entries.Count)
            {
                return _entries[_currentIndex];
            }
            return null;
        }
    }
}
