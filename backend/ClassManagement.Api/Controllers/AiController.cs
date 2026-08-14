using ClassManagement.Api.DTOs.Ai;
using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.Helpers;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiInsightService _aiInsightService;

    public AiController(IAiInsightService aiInsightService) => _aiInsightService = aiInsightService;

    /// <summary>
    /// Auto Attendance Insight — predicts low-attendance students and returns warnings/suggestions.
    /// </summary>
    [HttpGet("attendance-insights")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<AttendanceInsightResponse>>> GetAttendanceInsights([FromQuery] string? classId = null)
    {
        var result = await _aiInsightService.GetAttendanceInsightsAsync(classId, User.GetUserId(), User.GetUserRole());
        return Ok(ApiResponse<AttendanceInsightResponse>.Ok(result, "AI attendance insights generated."));
    }

    /// <summary>
    /// Smart Marks Feedback — generates performance feedback for a student mark.
    /// </summary>
    [HttpPost("marks-feedback/{markId}")]
    public async Task<ActionResult<ApiResponse<MarksFeedbackResponse>>> GenerateMarksFeedback(string markId)
    {
        var result = await _aiInsightService.GenerateMarksFeedbackAsync(markId, User.GetUserId(), User.GetUserRole());
        return Ok(ApiResponse<MarksFeedbackResponse>.Ok(result, "Smart feedback generated."));
    }
}
