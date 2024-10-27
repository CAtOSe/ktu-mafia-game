namespace Mafia.Server.Models.Decorator
{
    public class DeathAnnouncementDecorator : Decorator
    {
        public DeathAnnouncementDecorator(MorningAnnouncer wrappee) : base(wrappee) { }

        public override void Announce(List<Player> currentPlayers, List<Player> playersWhoDied)
        {
            base.Announce(currentPlayers, playersWhoDied);
            Console.WriteLine("DeathAnnouncementDecorator triggered.");
        }
    }
}
