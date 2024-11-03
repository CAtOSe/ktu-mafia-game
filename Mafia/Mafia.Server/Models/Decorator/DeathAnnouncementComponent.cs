namespace Mafia.Server.Models.Decorator
{
    public class DeathAnnouncementComponent : MorningAnnouncer
    {
        public DeathAnnouncementComponent() { }

        public override void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
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
            var votingResultMessage = new ChatMessage("", votedOff.Name + " has been voted off by the town.", "everyone", "dayNotification");
            votingResults.Add(votingResultMessage);
        }
    }
}
