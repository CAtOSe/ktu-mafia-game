namespace Mafia.Server.Models.Decorator
{
    public class RoleAnnouncementDecorator : Decorator
    {
        public RoleAnnouncementDecorator(MorningAnnouncer wrappee) : base(wrappee) { }

        public override void Announce(List<Player> currentPlayers, List<Player> playersWhoDied)
        {
            base.Announce(currentPlayers, playersWhoDied);
            Console.WriteLine("RoleAnnouncementDecorator triggered.");
        }
    }
}
