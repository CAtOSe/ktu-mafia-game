using Mafia.Server.Logging;
using LogLevel = Mafia.Server.Logging.LogLevel;

namespace Mafia.Server.Models.Decorator
{
    public class MorningAnnouncer
    {
        private GameLogger _logger = GameLogger.Instance;
        
        public virtual void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
            _logger.Log(LogLevel.Debug, "DayStartAnnouncements");
        }
        public virtual void VotingEnd(Player votedOffPlayer, List<ChatMessage> votingResults)
        {
            _logger.Log(LogLevel.Debug, "VotingEndAnnouncements");
        }
    }
}
