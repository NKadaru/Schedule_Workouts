using Microsoft.AspNetCore.Mvc;
using WorkoutScheduler.Api.Models;

namespace WorkoutScheduler.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var response = new HealthResponse();
        return Ok(response);
    }
}
