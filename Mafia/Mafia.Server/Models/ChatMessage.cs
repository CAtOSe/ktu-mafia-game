using System.Text.Json.Serialization;

namespace Mafia.Server.Models;

public class ChatMessage
{
    [JsonPropertyName("sender")]
    public string? Sender { get; set; } // Optional, since server messages may not have a sender

    [JsonPropertyName("content")]
    public string Content { get; set; } // Required

    [JsonPropertyName("recipient")]
    public string? Recipient { get; set; } // Optional, if sending to everyone, is left empty

    [JsonPropertyName("timeSent")]
    public double? TimeSent { get; set; } // Time in seconds since the game started (optional)

    [JsonPropertyName("category")]
    public ChatCategory ChatCategory { get; set; } // Required, type/category of the message
    

    public ChatMessage(string sender, string content, string recipient, string category)
    {
        Sender = sender;
        Content = content;
        Recipient = recipient;
        TimeSent = 0;
        ChatCategory = (ChatCategory)Enum.Parse(typeof(ChatCategory), category, true);
    }
}