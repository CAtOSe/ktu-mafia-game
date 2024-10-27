namespace Mafia.Server.Models.Decorator
{
    public class Decorator : MorningAnnouncer
    {
        protected MorningAnnouncer wrappee;

        public Decorator(MorningAnnouncer layer)
        {
            wrappee = layer;
        }

        public override void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
            wrappee.DayStartAnnouncements(currentPlayers, playersWhoDied, dayAnnouncements);
        }

        public override void VotingEnd(Player votedOff, List<ChatMessage> votingResults)
        {
            wrappee.VotingEnd(votedOff, votingResults);
        }
    }
}
