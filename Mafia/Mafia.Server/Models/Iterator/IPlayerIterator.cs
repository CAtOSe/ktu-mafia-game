using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.Iterator
{
    public interface IPlayerIterator
    {
        Player First();
        Player Next();
        bool IsDone();
        Player Current();
    }
}
