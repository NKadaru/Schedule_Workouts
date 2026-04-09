namespace WorkoutScheduler.Api.Models;

public class HealthResponse
{
    public string Status { get; set; } = "healthy";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
