using Microsoft.AspNetCore.Mvc;
using WorkoutScheduler.Api.Services;

namespace WorkoutScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WhoopController : ControllerBase
{
    private readonly WhoopService _whoopService;
    private readonly string _frontendRedirect;

    public WhoopController(WhoopService whoopService, IConfiguration config)
    {
        _whoopService = whoopService;
        _frontendRedirect = config["Whoop:FrontendRedirect"] ?? "http://localhost:4200";
    }

    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        return Ok(new { url = _whoopService.GetAuthorizeUrl() });
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string? state)
    {
        var success = await _whoopService.ExchangeCodeAsync(code);
        if (success)
        {
            return Redirect($"{_frontendRedirect}?whoop=connected");
        }
        return Redirect($"{_frontendRedirect}?whoop=error");
    }

    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new { connected = _whoopService.IsConnected });
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        if (!_whoopService.IsConnected)
            return Unauthorized(new { message = "WHOOP not connected" });

        var dashboard = await _whoopService.GetDashboardAsync();
        return Ok(dashboard);
    }

    [HttpGet("debug")]
    public async Task<IActionResult> Debug()
    {
        if (!_whoopService.IsConnected)
            return Unauthorized(new { message = "WHOOP not connected" });

        var debug = await _whoopService.GetDebugDataAsync();
        return Ok(debug);
    }
}
