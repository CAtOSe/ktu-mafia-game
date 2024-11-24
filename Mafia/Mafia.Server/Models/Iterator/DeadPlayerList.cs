namespace Mafia.Server.Models.Iterator
{
    public class DeadPlayerList : IPlayerAggregator
    {
        private readonly List<Player> _entries;

        public DeadPlayerList(IEnumerable<Player> entries)
        {
            _entries = entries.ToList();
        }

        public IPlayerIterator CreateIterator()
        {
            return new EvilPlayerIterator(_entries);
        }

    }
}
