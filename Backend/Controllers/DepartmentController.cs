using Backend.Common;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Defines an API controller that provides endpoints for department lookups.
/// </summary>
[ApiController]
[Route("api/[Controller]")]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    /// <summary>
    /// Retrieves all departments.
    /// </summary>
    [HttpGet(template: "GetAll")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DepartmentDto>>>> GetAll()
    {
        var result = await departmentService.GetAllAsync();
        return Ok(new ApiResponse<IReadOnlyList<DepartmentDto>>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Creates a new department.
    /// </summary>
    [HttpPost(template: "Create")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> Create([FromBody] CreateDepartmentDto department)
    {
        var result = await departmentService.CreateAsync(department);
        var response = new ApiResponse<DepartmentDto>(StatusCodes.Status201Created, result);
        return CreatedAtAction(nameof(GetAll), response);
    }
}
