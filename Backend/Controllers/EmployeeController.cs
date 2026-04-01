using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    /// <summary>
    /// Represents the controller for managing Employee resources in the Employee Management application.
    /// Provides endpoints for CRUD operations on employees using a database.
    /// </summary>
    [ApiController]
    [Route("api/[Controller]")]
    public class EmployeeController(EmployeeContext context) : ControllerBase
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
        [HttpGet(template: "GetAll")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBySalary = null,
            [FromQuery] string? sortByName = null,
            [FromQuery] string? department = null,
            [FromQuery] string? position = null)
        {
            // Fix for possible pageNumber and pageSize negative values
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "pageNumber and pageSize must be greater than 0." });
            }

            IQueryable<Employee> query = context.Employees;

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(employee => employee.Department == department);
            }

            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(employee => employee.Position == position);
            }

            bool sorted = false;

            // Salary sorting
            if (!string.IsNullOrEmpty(sortBySalary))
            {
                switch (sortBySalary.ToLowerInvariant())
                {
                    case "asc":
                        query = query.OrderBy(employee => employee.Salary);
                        sorted = true;
                        break;
                    case "desc":
                        query = query.OrderByDescending(employee => employee.Salary);
                        sorted = true;
                        break;
                    default:
                        return BadRequest(new { message = "sortBySalary must be 'asc' or 'desc'." });
                }
            }

            // Name sorting
            if (!string.IsNullOrEmpty(sortByName))
            {
                switch (sortByName.ToLowerInvariant())
                {
                    case "asc":
                        query = sorted
                            ? ((IOrderedQueryable<Employee>)query).ThenBy(employee => employee.Name)
                            : query.OrderBy(employee => employee.Name);
                        break;
                    case "desc":
                        query = sorted
                            ? ((IOrderedQueryable<Employee>)query).ThenByDescending(employee => employee.Name)
                            : query.OrderByDescending(employee => employee.Name);
                        break;
                    default:
                        return BadRequest(new { message = "sortByName must be 'asc' or 'desc'." });
                }
            }

            if (!sorted && string.IsNullOrEmpty(sortByName))
            {
                query = query.OrderBy(employee => employee.Id);
            }


            // Get total count for pagination BEFORE paging
            var totalEmployees = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);


            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                Items = employees,
                TotalPages = totalPages
            });
        }

        /// <summary>
        /// Retrieves an employee by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>The employee with the specified ID, 
        /// or a 404 Not Found response if the employee does not exist.</returns>
        [HttpGet(template: "Get{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found.", id });

            return Ok(employee);
        }

        /// <summary>
        /// Creates a new employee and adds it to the database.
        /// </summary>
        /// <param name="employee">The employee object to create.</param>
        /// <returns>The created employee and a location header with the URI of the new resource.</returns>
        [HttpPost(template: "Post")]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
            employee.Id = 0;
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, employee);
        }

        /// <summary>
        /// Updates the details of an existing employee.
        /// </summary>
        /// <param name="id">The ID of the employee to update.</param>
        /// <param name="updatedEmployee">The updated employee object.</param>
        /// <returns>The updated employee object,
        /// or a 404 Not Found response if the employee does not exist.</returns>
        [HttpPut(template: "Put{id}")]
        public async Task<ActionResult<Employee>> Update(int id, Employee updatedEmployee)
        {
            var employee = await context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            employee.Name = updatedEmployee.Name;
            employee.Position = updatedEmployee.Position;
            employee.Department = updatedEmployee.Department;
            employee.Salary = updatedEmployee.Salary;

            await context.SaveChangesAsync();

            return Ok(employee);
        }

        /// <summary>
        /// Deletes an employee by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <returns>A 204 No Content response if the deletion is successful, 
        /// or a 404 Not Found response if the employee does not exist.</returns>
        [HttpDelete(template: "Delete{id}")]
        public async Task<ActionResult<Employee>> Delete(int id)
        {
            var employee = await context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found", id });

            context.Employees.Remove(employee);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
