namespace Mafia.Server.Models.Iterator
{
    public interface IPlayerAggregator
    {
        IPlayerIterator CreateIterator();
    }
}
