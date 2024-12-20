﻿namespace Mafia.Server.Models.Iterator
{
    public class GoodPlayerList : IPlayerAggregator
    {
        private readonly List<Player> _entries;

        public GoodPlayerList(IEnumerable<Player> entries)
        {
            _entries = entries.ToList();
        }

        public IPlayerIterator CreateIterator()
        {
            return new GoodPlayerIterator(_entries);
        }

    }
}
