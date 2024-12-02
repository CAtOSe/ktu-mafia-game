using Mafia.Server.Models.Adapter;
using Mafia.Server.Models.Decorator;

namespace Mafia.Server.Models.ChainOfResponsibility
{
    public class HandlerContext
    {
        public bool _isDayPhase;
        public int _phaseCounter;
        public List<Player> _currentPlayers;
        public List<Player> _playersWhoDiedInTheNight;
        public List<ChatMessage> _dayStartAnnouncements;
        public MorningAnnouncer _morningAnnouncer;
        public IChatServiceAdapter _chatAdapter;

        public HandlerContext(bool isDayPhase, int phaseCounter, List<Player> currentPlayers, List<Player> playersWhoDiedInTheNight,
            List<ChatMessage> dayStartAnnouncements, MorningAnnouncer morningAnnouncer, IChatServiceAdapter chatAdapter)
        {
            _isDayPhase = isDayPhase;
            _phaseCounter = phaseCounter;
            _currentPlayers = currentPlayers;
            _playersWhoDiedInTheNight = playersWhoDiedInTheNight;
            _dayStartAnnouncements = dayStartAnnouncements;
            _morningAnnouncer = morningAnnouncer;
            _chatAdapter = chatAdapter;
        }
    }
}
