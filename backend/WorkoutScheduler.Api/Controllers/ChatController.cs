using Microsoft.AspNetCore.Mvc;
using WorkoutScheduler.Api.Models;
using WorkoutScheduler.Api.Services;

namespace WorkoutScheduler.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly GeminiAgentService _agentService;

    public ChatController(GeminiAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { message = "Message is required" });

        var reply = await _agentService.ChatAsync(request.Message, request.History);
        return Ok(new ChatResponse { Reply = reply });
    }
}
