namespace Mafia.Server.Models.Decorator
{
    public class DesignPatternIndicatorDecorator : Decorator
    {
        public DesignPatternIndicatorDecorator(MorningAnnouncer wrappee) : base(wrappee) { }

        public override void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
            base.DayStartAnnouncements(currentPlayers, playersWhoDied, dayAnnouncements);
            var dayAnnouncement = new ChatMessage("", "DECORATOR:", "everyone", "server");
            dayAnnouncements.Add(dayAnnouncement);
            Console.WriteLine("DesignPatternIndicatorDecorator triggered.");
        }
    }
}
