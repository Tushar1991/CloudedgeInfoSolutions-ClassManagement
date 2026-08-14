using ClassManagement.Api.DTOs.Common;
using ClassManagement.Api.DTOs.Marks;
using ClassManagement.Api.Helpers;
using ClassManagement.Api.Models.Enums;
using ClassManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MarksController : ControllerBase
{
    private readonly IMarkService _markService;

    public MarksController(IMarkService markService) => _markService = markService;

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<MarkDto>>> Create([FromBody] CreateMarkRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<MarkDto>.Fail("Validation failed."));

        var result = await _markService.CreateAsync(request, User.GetUserId());
        return Ok(ApiResponse<MarkDto>.Ok(result, "Mark recorded."));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<MarkDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? classId = null,
        [FromQuery] string? studentId = null)
    {
        var result = await _markService.GetPagedAsync(page, pageSize, classId, studentId, User.GetUserId(), User.GetUserRole());
        return Ok(ApiResponse<PagedResult<MarkDto>>.Ok(result));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<MarkDto>>> Update(string id, [FromBody] UpdateMarkRequest request)
    {
        var result = await _markService.UpdateAsync(id, request);
        return Ok(ApiResponse<MarkDto>.Ok(result!, "Mark updated."));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(string id)
    {
        var deleted = await _markService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Mark not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Mark deleted."));
    }
}
