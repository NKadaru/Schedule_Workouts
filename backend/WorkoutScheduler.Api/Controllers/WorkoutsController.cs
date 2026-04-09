using Microsoft.AspNetCore.Mvc;
using WorkoutScheduler.Api.Services;

namespace WorkoutScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutsController : ControllerBase
{
    private readonly WorkoutService _workoutService;

    public WorkoutsController(WorkoutService workoutService)
    {
        _workoutService = workoutService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_workoutService.GetAll());
    }

    [HttpGet("{day}")]
    public IActionResult GetByDay(string day)
    {
        var workouts = _workoutService.GetByDay(day);
        if (workouts.Count == 0)
            return NotFound(new { message = $"No workouts found for {day}" });

        return Ok(workouts);
    }
}
