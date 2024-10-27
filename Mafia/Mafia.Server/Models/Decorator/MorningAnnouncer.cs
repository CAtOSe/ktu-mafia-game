namespace Mafia.Server.Models.Decorator
{
    public class MorningAnnouncer
    {
        public virtual void Announce(List<Player> currentPlayers, List<Player> playersWhoDied)
        {
            Console.WriteLine("Good morning, everyone!");
        }
    }
}
