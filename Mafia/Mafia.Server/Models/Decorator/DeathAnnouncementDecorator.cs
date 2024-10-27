namespace Mafia.Server.Models.Decorator
{
    public class DeathAnnouncementDecorator : Decorator
    {
        public DeathAnnouncementDecorator(MorningAnnouncer wrappee) : base(wrappee) { }

        public override void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
            base.DayStartAnnouncements(currentPlayers, playersWhoDied, dayAnnouncements);
            foreach (var player in playersWhoDied)
            {
                var dayAnnouncement = new ChatMessage("", player.Name + " has died in the night.", "everyone", "dayNotification");
                dayAnnouncements.Add(dayAnnouncement);
            }
            if (playersWhoDied.Count == 0)
            {
                var dayAnnouncement = new ChatMessage("", "No one has died in the night.", "everyone", "dayNotification");
                dayAnnouncements.Add(dayAnnouncement);
            }
            Console.WriteLine("DeathAnnouncementDecorator triggered.");
        }
        public override void VotingEnd(Player votedOff, List<ChatMessage> votingResults)
        {
            base.VotingEnd(votedOff, votingResults);
            var votingResultMessage = new ChatMessage("", votedOff.Name + " has been voted off by the town.", "everyone", "dayNotification");
            votingResults.Add(votingResultMessage);
        }
    }
}
