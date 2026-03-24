using employeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employeeManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public EmployeeController(EmployeeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET /api/employees
        /// </summary>
        /// <returns>An enumerable collection of all employees. The collection will be empty if no employees are available.</returns>
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
        /// GET /api/employees/{id}
        /// </summary>
        /// <returns>The employee of the requested id.</returns>
        [HttpGet("/api/employees/{id}")]
        public async Task<ActionResult<Employee>> GetById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee not found.", id });

            return employee;
        }

        /// <summary>
        /// Creates a new employee and adds it to the collection.
        /// </summary>
        /// <returns>An ActionResult containing the created employee and a location header with the URI of the new resource.</returns>
        [HttpPost("/api/employees")]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        /// <summary>
        /// Updates employee data of requested id
        /// </summary>
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
        /// Deletes employee of requested id
        /// </summary>
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
