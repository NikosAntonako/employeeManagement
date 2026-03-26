using employeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employeeManagement.Controllers
{
    /// <summary>
    /// Represents the controller for managing Employee resources in the Employee Management application.
    /// Provides endpoints for CRUD operations on employees.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeController"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing employee data.</param>
        public EmployeeController(EmployeeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a paginated list of all employees.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
        /// <param name="pageSize">The number of employees per page (default is 10).</param>
        /// <returns>An enumerable collection of employees. Returns an empty collection if no employees are available.</returns>
        [HttpGet("/api/employees")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest(new { message = "pageNumber and pageSize must be greater than 0" });

            var pagedEmployees = await _context.Employees
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(pagedEmployees);
        }

        /// <summary>
        /// Retrieves an employee by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee.</param>
        /// <returns>The employee with the specified ID, or a 404 Not Found response if the employee does not exist.</returns>
        [HttpGet("/api/employees/{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found.", id });

            return employee;
        }

        /// <summary>
        /// Creates a new employee and adds it to the database.
        /// </summary>
        /// <param name="employee">The employee object to create.</param>
        /// <returns>The created employee and a location header with the URI of the new resource.</returns>
        [HttpPost("/api/employees")]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        /// <summary>
        /// Updates the details of an existing employee.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to update.</param>
        /// <param name="updatedEmployee">The updated employee object.</param>
        /// <returns>The updated employee object, or a 404 Not Found response if the employee does not exist.</returns>
        [HttpPut("/api/employees/{id}")]
        public async Task<ActionResult<Employee>> Update(int id, Employee updatedEmployee)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            employee.Name = updatedEmployee.Name;
            employee.Position = updatedEmployee.Position;
            employee.Department = updatedEmployee.Department;
            employee.Salary = updatedEmployee.Salary;

            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        /// <summary>
        /// Deletes an employee by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the employee to delete.</param>
        /// <returns>A 204 No Content response if the deletion is successful, or a 404 Not Found response if the employee does not exist.</returns>
        [HttpDelete("/api/employees/{id}")]
        public async Task<ActionResult<Employee>> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found", id });

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
