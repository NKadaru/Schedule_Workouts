using System.Text.Json;
using WorkoutScheduler.Api.Models;

namespace WorkoutScheduler.Api.Services;

public class WorkoutService(IWebHostEnvironment env)
{
    private readonly string _filePath = Path.Combine(env.ContentRootPath, "Data", "workouts.json");
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Dictionary<string, DayPlan> GetAll()
    {
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<Dictionary<string, DayPlan>>(json, _jsonOptions)
               ?? new Dictionary<string, DayPlan>();
    }

    public DayPlan? GetByDay(string day)
    {
        var all = GetAll();
        return all.TryGetValue(day, out var plan) ? plan : null;
    }
}
