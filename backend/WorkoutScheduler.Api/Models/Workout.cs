namespace WorkoutScheduler.Api.Models;

public class Workout
{
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class DayPlan
{
    public string Quote { get; set; } = string.Empty;
    public List<Workout> Exercises { get; set; } = new();
}
