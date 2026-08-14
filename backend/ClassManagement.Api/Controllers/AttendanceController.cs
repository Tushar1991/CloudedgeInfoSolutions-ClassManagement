using ClassManagement.Api.DTOs.Attendance;
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
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService) => _attendanceService = attendanceService;

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<AttendanceDto>>> Mark([FromBody] MarkAttendanceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<AttendanceDto>.Fail("Validation failed."));

        var result = await _attendanceService.MarkAsync(request, User.GetUserId());
        return Ok(ApiResponse<AttendanceDto>.Ok(result, "Attendance marked."));
    }

    [HttpPost("bulk")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AttendanceDto>>>> BulkMark([FromBody] BulkAttendanceRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<IReadOnlyList<AttendanceDto>>.Fail("Validation failed."));

        var result = await _attendanceService.BulkMarkAsync(request, User.GetUserId());
        return Ok(ApiResponse<IReadOnlyList<AttendanceDto>>.Ok(result, "Bulk attendance saved."));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AttendanceDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? classId = null,
        [FromQuery] string? studentId = null)
    {
        var result = await _attendanceService.GetPagedAsync(page, pageSize, classId, studentId, User.GetUserId(), User.GetUserRole());
        return Ok(ApiResponse<PagedResult<AttendanceDto>>.Ok(result));
    }
}
