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

    public Dictionary<string, List<Workout>> GetAll()
    {
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<Dictionary<string, List<Workout>>>(json, _jsonOptions)
               ?? new Dictionary<string, List<Workout>>();
    }

    public List<Workout> GetByDay(string day)
    {
        var all = GetAll();
        return all.TryGetValue(day, out var workouts) ? workouts : new List<Workout>();
    }
}
