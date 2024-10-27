namespace Mafia.Server.Models.Decorator
{
    public class NightActionsSummaryDecorator : Decorator
    {
        public NightActionsSummaryDecorator(MorningAnnouncer wrappee) : base(wrappee) { }

        public override void Announce(List<Player> currentPlayers, List<Player> playersWhoDied)
        {
            base.Announce(currentPlayers, playersWhoDied);
            Console.WriteLine("NightActionsSummaryDecorator triggered.");
        }
    }
}
