using Backend.Services;
using EmployeeManagement.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Provides endpoints for querying and creating departments.
/// </summary>
[ApiController]
[Route("api/[Controller]")]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    /// <summary>
    /// Retrieves all departments.
    /// </summary>
    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DepartmentDto>>>> GetAllAsync()
    {
        var result = await departmentService.GetAllAsync();
        return Ok(new ApiResponse<IReadOnlyList<DepartmentDto>>(StatusCodes.Status200OK, result));
    }
}
