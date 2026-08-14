using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.DTOs.Enrollments;
using ClassManagement.Api.Helpers;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService) => _enrollmentService = enrollmentService;

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Create([FromBody] CreateEnrollmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<EnrollmentDto>.Fail("Validation failed."));

        var result = await _enrollmentService.CreateAsync(request);
        return Ok(ApiResponse<EnrollmentDto>.Ok(result, "Student enrolled."));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<EnrollmentDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? classId = null,
        [FromQuery] string? studentId = null)
    {
        var result = await _enrollmentService.GetPagedAsync(page, pageSize, classId, studentId, User.GetUserId(), User.GetUserRole());
        return Ok(ApiResponse<PagedResult<EnrollmentDto>>.Ok(result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(string id)
    {
        var deleted = await _enrollmentService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Enrollment not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Enrollment removed."));
    }
}
