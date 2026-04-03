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
    /// <returns>An asynchronous operation that returns an HTTP action result containing a paged list of employee data transfer objects.</returns>
    [HttpGet(template: "GetAll")]
    public async Task<ActionResult<PagedResultDto>> GetAll([FromQuery] EmployeeQueryDto employeeQuery)
    {
        var result = await employeeService.GetAllAsync(employeeQuery);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the employee details for the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to retrieve. Must be a positive integer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="ActionResult{T}">ActionResult</see> of <see cref="EmployeeResponseDto"/> containing the employee details.</returns>
    [HttpGet(template: "GetById/{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById([FromRoute] int id)
    {
        var result = await employeeService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new employee record using the provided employee data.
    /// </summary>
    /// <param name="employee">The employee data to create. Must not be null.</param>
    /// <returns>An asynchronous operation that returns an ActionResult containing the created employee information.</returns>
    [HttpPost(template: "Create")]
    public async Task<ActionResult<EmployeeResponseDto>> Create([FromBody] EmployeeDto employee)
    {
        var result = await employeeService.CreateAsync(employee);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates the details of an existing employee with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to update.</param>
    /// <param name="updatedEmployee">An object containing the updated employee information. Cannot be null.</param>
    /// <returns>An ActionResult containing the updated employee data.</returns>
    [HttpPut(template: "Update/{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> Update([FromRoute] int id, [FromBody] EmployeeDto updatedEmployee)
    {
        var result = await employeeService.UpdateAsync(id, updatedEmployee);
        return Ok(result);
    }

    /// <summary>
    /// Deletes the resource identified by the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the resource to delete. Must be a positive integer.</param>
    /// <returns>An ActionResult that indicates the outcome of the delete operation.</returns>
    [HttpDelete(template: "Delete/{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await employeeService.DeleteAsync(id);
        return NoContent();
    }
}