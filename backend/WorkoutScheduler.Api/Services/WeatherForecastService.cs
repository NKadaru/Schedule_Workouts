using WorkoutScheduler.Api.Models;

namespace WorkoutScheduler.Api.Services;

public class WeatherForecastService
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public WeatherForecast[] GetForecasts()
    {
        var random = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = random.Next(-20, 55),
            Summary = Summaries[random.Next(Summaries.Length)]
        }).ToArray();
    }
}
