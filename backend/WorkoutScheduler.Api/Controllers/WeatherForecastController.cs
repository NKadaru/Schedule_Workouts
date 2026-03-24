using Microsoft.AspNetCore.Mvc;
using WorkoutScheduler.Api.Models;
using WorkoutScheduler.Api.Services;

namespace WorkoutScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly WeatherForecastService _weatherForecastService;

    public WeatherForecastController(WeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var forecasts = _weatherForecastService.GetForecasts();
        return Ok(forecasts);
    }
}
