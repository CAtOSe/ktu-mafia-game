using System.Numerics;

namespace Mafia.Server.Models.Decorator
{
    public class AlignmentAnnouncementDecorator : Decorator
    {

        public AlignmentAnnouncementDecorator(MorningAnnouncer wrappee) : base(wrappee) { }

        public override void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
            base.DayStartAnnouncements(currentPlayers, playersWhoDied, dayAnnouncements);
            foreach (var player in playersWhoDied)
            {
                var dayAnnouncement = new ChatMessage("", player.Name + " was " + player.Role.Alignment + ".", "everyone", "dayNotification");
                dayAnnouncements.Add(dayAnnouncement);
            }
            Console.WriteLine("RoleAnnouncementDecorator triggered.");
        }

        public override void VotingEnd(Player votedOff, List<ChatMessage> votingResults)
        {
            base.VotingEnd(votedOff, votingResults);
            var votingResultMessage = new ChatMessage("", votedOff.Name + " was " + votedOff.Role.Alignment + ".", "everyone", "dayNotification");
            votingResults.Add(votingResultMessage);
        }
    }
}
