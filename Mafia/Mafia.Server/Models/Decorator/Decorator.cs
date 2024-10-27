namespace Mafia.Server.Models.Decorator
{
    public class Decorator : MorningAnnouncer
    {
        protected MorningAnnouncer wrappee;

        public Decorator(MorningAnnouncer layer)
        {
            wrappee = layer;
        }

        public override void Announce(List<Player> currentPlayers, List<Player> playersWhoDied)
        {
            wrappee.Announce(currentPlayers, playersWhoDied);
        }
    }
}
