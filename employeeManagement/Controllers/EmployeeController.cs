using employeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employeeManagement.Controllers
{
    /// <summary>
    /// Represents the controller for managing Employee resources in the Employee Management application.
    /// Provides endpoints for CRUD operations on employees using a database.
    /// </summary>
    [ApiController]
    [Route("api/employees")]
    public class EmployeeController(EmployeeContext context) : ControllerBase
    {
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
        [HttpGet]
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
                query = query.Where(e => e.Department == department);
            }

            if (!string.IsNullOrEmpty(position))
            {
                query = query.Where(e => e.Position == position);
            }

            bool sorted = false;

            if (!string.IsNullOrEmpty(sortBySalary))
            {
                if (!sortBySalary.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
                    !sortBySalary.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "sortBySalary must 'asc' or 'desc'." });
                }
                query = sortBySalary.Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => e.Salary)
                    : query.OrderBy(e => e.Salary);
                sorted = true;
            }

            if (!string.IsNullOrEmpty(sortByName))
            {
                if (!sortByName.Equals("asc", StringComparison.CurrentCultureIgnoreCase) &&
                    !sortByName.Equals("desc", StringComparison.CurrentCultureIgnoreCase))
                {
                    return BadRequest(new { message = "sortByName must have 'asc' or 'desc'." });
                }
                if (sorted)
                {
                    query = sortByName.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                        ? ((IOrderedQueryable<Employee>)query).ThenByDescending(e => e.Name)
                        : ((IOrderedQueryable<Employee>)query).ThenBy(e => e.Name);
                }
                else
                {
                    query = sortByName.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                        ? query.OrderByDescending(e => e.Name)
                        : query.OrderBy(e => e.Name);
                }
            }

            var employees = await query
                // For page 1, you want to skip 0 items: (1 - 1) * pageSize = 0 etc
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return employees;
        }

        /// <summary>
        /// Retrieves an employee by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>The employee with the specified ID, 
        /// or a 404 Not Found response if the employee does not exist.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found.", id });

            return employee;
        }

        /// <summary>
        /// Creates a new employee and adds it to the database.
        /// </summary>
        /// <param name="employee">The employee object to create.</param>
        /// <returns>The created employee and a location header with the URI of the new resource.</returns>
        [HttpPost]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
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
        [HttpPut("{id}")]
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
        [HttpDelete("{id}")]
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
