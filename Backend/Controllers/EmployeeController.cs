using Backend.Common;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Defines an API controller that provides endpoints for managing employee records, including operations to create,
/// retrieve, update, and delete employees.
/// </summary>
[ApiController]
[Route("api/[Controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    /// <summary>
    /// Retrieves a paged list of employees that match the specified query parameters.
    /// </summary>
    [HttpGet(template: "GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResultDto>>> GetAll([FromQuery] EmployeeQueryDto employeeQuery)
    {
        var result = await employeeService.GetAllAsync(employeeQuery);
        return Ok(new ApiResponse<PagedResultDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Retrieves the employee details for the specified identifier.
    /// </summary>
    [HttpGet(template: "GetById/{id:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> GetById([FromRoute] int id)
    {
        var result = await employeeService.GetByIdAsync(id);
        return Ok(new ApiResponse<EmployeeResponseDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Creates a new employee record using the provided employee data.
    /// </summary>
    [HttpPost(template: "Create")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> Create([FromBody] EmployeeDto employee)
    {
        var result = await employeeService.CreateAsync(employee);
        var response = new ApiResponse<EmployeeResponseDto>(StatusCodes.Status201Created, result);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
    }

    /// <summary>
    /// Updates the details of an existing employee with the specified identifier.
    /// </summary>
    [HttpPut(template: "Update/{id:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> Update([FromRoute] int id, [FromBody] EmployeeDto updatedEmployee)
    {
        var result = await employeeService.UpdateAsync(id, updatedEmployee);
        return Ok(new ApiResponse<EmployeeResponseDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Deletes the resource identified by the specified ID.
    /// </summary>
    [HttpDelete("Delete/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete([FromRoute] int id)
    {
        await employeeService.DeleteAsync(id);
        return Ok(new ApiResponse<object>(StatusCodes.Status200OK, null));
    }
}