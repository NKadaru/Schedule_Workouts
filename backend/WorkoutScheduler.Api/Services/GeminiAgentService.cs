using System.Text.Json;
using WorkoutScheduler.Api.Models;

namespace WorkoutScheduler.Api.Services;

public class GeminiAgentService(
    HttpClient httpClient,
    WorkoutService workoutService,
    WhoopService whoopService,
    ILogger<GeminiAgentService> logger,
    IConfiguration config)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly WorkoutService _workoutService = workoutService;
    private readonly WhoopService _whoopService = whoopService;
    private readonly ILogger<GeminiAgentService> _logger = logger;
    private readonly string _apiKey = config["Groq:ApiKey"] ?? "";
    private readonly string _model = config["Groq:Model"] ?? "llama-3.3-70b-versatile";

    public async Task<string> ChatAsync(string userMessage, List<ChatMessage> history)
    {
        var workoutContext = GetWorkoutContext();
        var whoopContext = await GetWhoopContext();
        var systemPrompt = BuildSystemPrompt(workoutContext, whoopContext);

        var messages = new List<Dictionary<string, string>>
        {
            new() { ["role"] = "system", ["content"] = systemPrompt }
        };

        foreach (var msg in history)
        {
            messages.Add(new Dictionary<string, string>
            {
                ["role"] = msg.Role == "user" ? "user" : "assistant",
                ["content"] = msg.Content
            });
        }

        messages.Add(new Dictionary<string, string>
        {
            ["role"] = "user",
            ["content"] = userMessage
        });

        var requestBody = new Dictionary<string, object>
        {
            ["model"] = _model,
            ["messages"] = messages,
            ["temperature"] = 0.7,
            ["max_tokens"] = 1024
        };

        var jsonPayload = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        _logger.LogInformation("Calling Groq: {Model}", _model);

        var response = await _httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Groq error: {Status} - {Body}", response.StatusCode, json);
            return $"Sorry, I couldn't process that. (API error: {response.StatusCode})";
        }

        return ExtractReply(json);
    }

    private string BuildSystemPrompt(string workoutContext, string whoopContext)
    {
        return $"""
            You are GrindFlow AI, a friendly fitness assistant.
            Help users with workout advice, form tips, nutrition, and their schedule.
            Use the WHOOP recovery data to make smart recommendations — if recovery is low (red/yellow), suggest lighter workouts or rest. If recovery is high (green), encourage pushing harder.
            
            Current weekly workout plan:
            {workoutContext}
            
            {whoopContext}
            
            When suggesting workout changes, consider the user's recovery score, strain, and sleep.
            Keep responses concise and friendly.
            """;
    }

    private async Task<string> GetWhoopContext()
    {
        if (!_whoopService.IsConnected)
            return "WHOOP is not connected — no recovery data available.";

        try
        {
            var dashboard = await _whoopService.GetDashboardAsync();
            var lines = new List<string>
            {
                "WHOOP data (today):",
                $"  Recovery: {dashboard.RecoveryScore}% ({dashboard.RecoveryLevel})",
                $"  Strain: {dashboard.Strain}",
                $"  HRV: {dashboard.Hrv} ms",
                $"  Resting Heart Rate: {dashboard.RestingHeartRate} bpm",
                $"  Sleep: {dashboard.SleepHours} hours ({dashboard.SleepPerformance}% performance)",
                "",
                "Recent daily history:"
            };

            foreach (var day in dashboard.DailyHistory.Take(7))
            {
                lines.Add($"  {day.Date}: Recovery {day.Recovery}% ({day.RecoveryLevel}), Strain {day.Strain}, Sleep {day.SleepHours} hrs");
            }

            return string.Join("\n", lines);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not fetch WHOOP data for chatbot");
            return "WHOOP data unavailable.";
        }
    }

    private string GetWorkoutContext()
    {
        var plans = _workoutService.GetAll();
        var lines = new List<string>();
        foreach (var (day, plan) in plans)
        {
            var exercises = string.Join(", ",
                plan.Exercises.Select(e => $"{e.Name} ({e.Sets}x{e.Reps})"));
            lines.Add($"{day}: {exercises}");
        }
        return string.Join("\n", lines);
    }

    private string ExtractReply(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var text = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
            return text ?? "I didn't get a response. Try again?";
        }
        catch
        {
            return "Something went wrong parsing the response. Try again?";
        }
    }
}
