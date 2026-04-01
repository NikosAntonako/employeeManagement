using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;
/// <summary>
/// Represents the controller for managing Employee resources in the Employee Management application.
/// Provides endpoints for CRUD operations on employees using a database.
/// </summary>
[ApiController]
[Route("api/[Controller]")]
public class EmployeeController(IEmployeeService service) : ControllerBase
{
    // Middleware Test
    [HttpGet("Throw")]
    public ActionResult Throw()
    {
        throw new Exception("Test exception from controller");
    }

    /// <summary>
    /// Retrieves all employees from the database with optional filtering, paging, and sorting.
    /// </summary>
    /// <param name="pageNumber">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of employees per page (default is 10).</param>
    /// <param name="sortBySalary">Sorts employees by salary ("asc" or "desc").</param>
    /// <param name="sortByName">Sorts employees by name ("asc" or "desc").</param>
    /// <param name="department">Optional filter by department.</param>
    /// <param name="position">Optional filter by position.</param>
    /// <returns>A paged, optionally filtered and sorted collection of employees.</returns>
    [HttpGet("GetAll")]
    public async Task<ActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBySalary = null,
        [FromQuery] string? sortByName = null,
        [FromQuery] string? department = null,
        [FromQuery] string? position = null,
        [FromQuery] string? searchTerm = null
        )
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(new { message = "pageNumber and pageSize must be greater than 0." });

        var (items, totalPages) = await service.GetAllAsync(
            pageNumber,
            pageSize,
            sortBySalary,
            sortByName,
            department,
            position,
            searchTerm
            );
        return Ok(new { Items = items, TotalPages = totalPages });
    }

    /// <summary>
    /// Retrieves an employee by their ID.
    /// </summary>
    /// <param name="id">The ID of the employee.</param>
    /// <returns>The employee with the specified ID, 
    /// or a 404 Not Found response if the employee does not exist.</returns>
    [HttpGet("Get{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var employee = await service.GetByIdAsync(id);
        if (employee == null)
            return NotFound(new { message = "Employee not found.", id });
        return Ok(employee);
    }

    /// <summary>
    /// Creates a new employee and adds it to the database.
    /// </summary>
    /// <param name="employee">The employee object to create.</param>
    /// <returns>The created employee and a location header with the URI of the new resource.</returns>
    [HttpPost("Post")]
    public async Task<ActionResult> Create(Employee employee)
    {
        var created = await service.CreateAsync(employee);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    /// <summary>
    /// Updates the details of an existing employee.
    /// </summary>
    /// <param name="id">The ID of the employee to update.</param>
    /// <param name="updatedEmployee">The updated employee object.</param>
    /// <returns>The updated employee object,
    /// or a 404 Not Found response if the employee does not exist.</returns>
    [HttpPut("Put{id}")]
    public async Task<ActionResult> Update(int id, Employee updatedEmployee)
    {
        var employee = await service.UpdateAsync(id, updatedEmployee);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    /// <summary>
    /// Deletes an employee by their ID.
    /// </summary>
    /// <param name="id">The ID of the employee to delete.</param>
    /// <returns>A 204 No Content response if the deletion is successful, 
    /// or a 404 Not Found response if the employee does not exist.</returns>
    [HttpDelete("Delete{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = "Employee not found", id });
        return NoContent();
    }
}
