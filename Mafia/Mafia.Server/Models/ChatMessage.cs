using System.Text.Json.Serialization;

namespace Mafia.Server.Models;

public class ChatMessage
{
    [JsonPropertyName("sender")]
    public string Sender { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } // Required

    [JsonPropertyName("recipient")]
    public string Recipient { get; set; } // Optional, if sending to everyone, is left empty

    [JsonPropertyName("timeSent")]
    public int TimeSent { get; set; } // Time in seconds since the game started

    [JsonPropertyName("category")]
    public string ChatCategory { get; set; } // Required, type/category of the message
    

    public ChatMessage(string sender, string content, string recipient, string category, int timeSent = 0)
    {
        Sender = sender;
        Content = content;
        Recipient = recipient;
        TimeSent = timeSent;
        ChatCategory = ((ChatCategory)Enum.Parse(typeof(ChatCategory), category, true)).ToString();
    }
}