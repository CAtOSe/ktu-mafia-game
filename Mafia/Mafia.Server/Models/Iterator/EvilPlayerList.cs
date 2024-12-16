using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.Iterator
{
    public class EvilPlayerList : IPlayerAggregator
    {
        private readonly List<Player> _entries;

        public EvilPlayerList(IEnumerable<Player> entries)
        {
            _entries = entries.ToList();
        }

        public IPlayerIterator GetIterator()
        {
            return new EvilPlayerIterator(_entries);
        }

    }
}
