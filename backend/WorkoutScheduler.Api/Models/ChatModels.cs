namespace WorkoutScheduler.Api.Models;

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatMessage> History { get; set; } = new();
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty; // "user" or "model"
    public string Content { get; set; } = string.Empty;
}

public class ChatResponse
{
    public string Reply { get; set; } = string.Empty;
}
