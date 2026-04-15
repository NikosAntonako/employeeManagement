using Backend.Services;
using EmployeeManagement.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Provides endpoints for querying and managing employees.
/// </summary>
[ApiController]
[Route("api/[Controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    /// <summary>
    /// Retrieves a paged list of employees that match the specified query parameters.
    /// </summary>
    [HttpGet("GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResultDto>>> GetAllAsync([FromQuery] EmployeeQueryDto employeeQuery)
    {
        var result = await employeeService.GetAllAsync(employeeQuery);
        return Ok(new ApiResponse<PagedResultDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Retrieves the employee details for the specified identifier.
    /// </summary>
    [HttpGet("GetById/{id:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> GetByIdAsync([FromRoute] int id)
    {
        var result = await employeeService.GetByIdAsync(id);
        return Ok(new ApiResponse<EmployeeResponseDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Creates a new employee record using the provided employee data.
    /// </summary>
    [HttpPost("Create")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> CreateAsync([FromBody] EmployeeDto employee)
    {
        if (employee.Name == "abc")
        {
            return BadRequest(new ApiResponse<EmployeeResponseDto>(StatusCodes.Status400BadRequest, null, "forbidden operation"));
        }

        var result = await employeeService.CreateAsync(employee);
        var response = new ApiResponse<EmployeeResponseDto>(StatusCodes.Status201Created, result);

        return CreatedAtAction("GetByIdAsync", new { id = result.Id }, response);
    }

    /// <summary>
    /// Updates the details of an existing employee with the specified identifier.
    /// </summary>
    [HttpPut("Update/{id:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> UpdateAsync([FromRoute] int id, [FromBody] EmployeeDto updatedEmployee)
    {
        var result = await employeeService.UpdateAsync(id, updatedEmployee);
        return Ok(new ApiResponse<EmployeeResponseDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Deletes the employee with the specified identifier.
    /// </summary>
    [HttpDelete("Delete/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAsync([FromRoute] int id)
    {
        await employeeService.DeleteAsync(id);
        return Ok(new ApiResponse<object>(StatusCodes.Status200OK, null));
    }
}