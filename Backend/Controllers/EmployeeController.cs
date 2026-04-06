using Backend.Common;
using Backend.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Defines an API controller that provides endpoints for managing employee records, including operations to create,
/// retrieve, update, and delete employees.
/// </summary>
/// <remarks>This controller exposes RESTful endpoints for employee management and relies on dependency injection
/// for the employee service. All endpoints return standard HTTP responses and support asynchronous operations. The
/// controller is intended to be used in an ASP.NET Core Web API application.</remarks>
/// <param name="employeeService">The employee service used to perform business operations related to employee data. Must not be null.</param>
[ApiController]
[Route("api/[Controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{
    /// <summary>
    /// Retrieves a paged list of employees that match the specified query parameters.
    /// </summary>
    /// <param name="employeeQuery">An object containing filtering, sorting, and paging options to apply to the employee list.</param>
    /// <returns>A standardized response wrapping the paged employee list with HTTP 200 OK status.</returns>
    [HttpGet(template: "GetAll")]
    public async Task<ActionResult<ApiResponse<PagedResultDto>>> GetAll([FromQuery] EmployeeQueryDto employeeQuery)
    {
        var result = await employeeService.GetAllAsync(employeeQuery);
        return Ok(new ApiResponse<PagedResultDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Retrieves the employee details for the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to retrieve. Must be a positive integer.</param>
    /// <returns>A standardized response wrapping the matched employee with HTTP 200 OK status.</returns>
    [HttpGet(template: "GetById/{id:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> GetById([FromRoute] int id)
    {
        var result = await employeeService.GetByIdAsync(id);
        return Ok(new ApiResponse<EmployeeResponseDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Creates a new employee record using the provided employee data.
    /// </summary>
    /// <param name="employee">The employee data to create. Must not be null.</param>
    /// <returns>A standardized 201 Created response wrapping the newly created employee.</returns>
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
    /// <param name="id">The unique identifier of the employee to update.</param>
    /// <param name="updatedEmployee">An object containing the updated employee information. Cannot be null.</param>
    /// <returns>A standardized response wrapping the updated employee with HTTP 200 OK status.</returns>
    [HttpPut(template: "Update/{id:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> Update([FromRoute] int id, [FromBody] EmployeeDto updatedEmployee)
    {
        var result = await employeeService.UpdateAsync(id, updatedEmployee);
        return Ok(new ApiResponse<EmployeeResponseDto>(StatusCodes.Status200OK, result));
    }

    /// <summary>
    /// Deletes the resource identified by the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the resource to delete. Must be a positive integer.</param>
    /// <returns>A standardized response confirming successful deletion with HTTP 200 OK status.</returns>
    [HttpDelete(template: "Delete/{id:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete([FromRoute] int id)
    {
        await employeeService.DeleteAsync(id);
        return Ok(new ApiResponse<bool>(StatusCodes.Status200OK));
    }
}