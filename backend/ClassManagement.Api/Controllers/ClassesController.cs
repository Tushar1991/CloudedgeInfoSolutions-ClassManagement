using ClassManagement.Api.DTOs.Classes;
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
public class ClassesController : ControllerBase
{
    private readonly IClassService _classService;

    public ClassesController(IClassService classService) => _classService = classService;

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<ClassDto>>> Create([FromBody] CreateClassRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ClassDto>.Fail("Validation failed."));

        var result = await _classService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ClassDto>.Ok(result, "Class created."));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ClassDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? teacherId = null)
    {
        var result = await _classService.GetPagedAsync(page, pageSize, teacherId, User.GetUserId(), User.GetUserRole());
        return Ok(ApiResponse<PagedResult<ClassDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ClassDto>>> GetById(string id)
    {
        var result = await _classService.GetByIdAsync(id);
        if (result is null) return NotFound(ApiResponse<ClassDto>.Fail("Class not found."));
        return Ok(ApiResponse<ClassDto>.Ok(result));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Teacher}")]
    public async Task<ActionResult<ApiResponse<ClassDto>>> Update(string id, [FromBody] UpdateClassRequest request)
    {
        var result = await _classService.UpdateAsync(id, request);
        return Ok(ApiResponse<ClassDto>.Ok(result!, "Class updated."));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(string id)
    {
        var deleted = await _classService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail("Class not found."));
        return Ok(ApiResponse<object>.Ok(new { }, "Class deleted."));
    }
}
