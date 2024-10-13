namespace Mafia.Server.Models;

public class ChatMessage
{
    public Player Player { get; set; }
    public string Content { get; set; }
    public string Recipient { get; set; }
    public DateTime TimeSent { get; set; }
    public ChatCategory ChatCategory { get; set; }
    

    public ChatMessage(Player player, string content, string recipient, string chatCategory)
    {
        Player = player;
        Content = content;
        Recipient = recipient;
        TimeSent = DateTime.Now;
        ChatCategory = (ChatCategory)Enum.Parse(typeof(ChatCategory), chatCategory, true);
    }
}