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
/// <param name="service">The employee service used to perform business operations related to employee data. Must not be null.</param>
[ApiController]
[Route("api/[controller]")]
public class EmployeeController(IEmployeeService service) : ControllerBase
{
    /// <summary>
    /// Throws a test exception to demonstrate error handling in the controller.
    /// </summary>
    /// <returns>This method does not return a value because it always throws an exception.</returns>
    /// <exception cref="Exception">Always thrown to simulate an error condition for testing purposes.</exception>
    [HttpGet(template: "ThrowException")]
    public IActionResult Throw()
    {
        throw new Exception("Test exception from controller");
    }

    /// <summary>
    /// Retrieves a paged list of employees that match the specified query parameters.
    /// </summary>
    /// <param name="query">An object containing filtering, sorting, and paging options to apply to the employee list.</param>
    /// <returns>An asynchronous operation that returns an HTTP action result containing a paged list of employee data transfer objects.</returns>
    [HttpGet(template: "GetAll")]
    public async Task<ActionResult<PagedResultDto<EmployeeResponseDto>>> GetAll([FromQuery] EmployeeQueryDto query)
    {
        var result = await service.GetAllAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the employee details for the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to retrieve. Must be a positive integer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see
    /// cref="ActionResult{T}">ActionResult</see> of <see cref="EmployeeResponseDto"/> containing the employee details if found;
    /// otherwise, an appropriate error response.</returns>
    [HttpGet(template: "Get/{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(int id)
    {
        var result = await service.GetByIdAsync(id);
        return result == null
            ? NotFound(new ProblemDetails
            {
                Title = "Employee not found.",
                Status = StatusCodes.Status404NotFound
            })
            : Ok(result);
    }

    /// <summary>
    /// Creates a new employee record using the provided employee data.
    /// </summary>
    /// <param name="employee">The employee data to create. Must not be null.</param>
    /// <returns>An asynchronous operation that returns an ActionResult containing the created employee information.</returns>
    [HttpPost(template: "Post")]
    public async Task<ActionResult<EmployeeResponseDto>> Create([FromBody] EmployeeDto employee)
    {
        var result = await service.CreateAsync(employee);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates the details of an existing employee with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to update.</param>
    /// <param name="updatedEmployee">An object containing the updated employee information. Cannot be null.</param>
    /// <returns>An ActionResult containing the updated employee data if the update is successful;
    /// otherwise, an appropriate error response.</returns>
    [HttpPut(template: "Put/{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> Update(int id, [FromBody] EmployeeDto updatedEmployee)
    {
        var result = await service.UpdateAsync(id, updatedEmployee);
        return result == null
            ? NotFound(new ProblemDetails
            {
                Title = "Employee not found.",
                Status = StatusCodes.Status404NotFound
            })
            : Ok(result);
    }

    /// <summary>
    /// Deletes the resource identified by the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the resource to delete. Must be a positive integer.</param>
    /// <returns>An ActionResult that indicates the outcome of the delete operation. Returns a success response if the resource
    /// was deleted; otherwise, returns an error response.</returns>
    [HttpDelete(template: "Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await service.DeleteAsync(id);
        return result
            ? NoContent()
            : NotFound(new ProblemDetails
            {
                Title = "Employee not found.",
                Status = StatusCodes.Status404NotFound
            });
    }
}
