using employeeManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace employeeManagement.Controllers
{
    /// <summary>
    /// Represents the controller for managing Employee resources in the Employee Management application.
    /// Provides endpoints for CRUD operations on employees using an in-memory collection.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        /// <summary>
        /// Static in-memory collection of employees.
        /// This collection is used to store employee data during the application's runtime.
        /// </summary>
        private static readonly List<Employee> Employees = new();

        /// <summary>
        /// Retrieves all employees from the in-memory collection.
        /// </summary>
        /// <returns>An enumerable collection of all employees. 
        /// The collection will be empty if no employees are available.</returns>
        [HttpGet("/api/employees")]
        public IEnumerable<Employee> GetAll()
        {
            return Employees;
        }

        /// <summary>
        /// Retrieves an employee by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee.</param>
        /// <returns>The employee with the specified ID, 
        /// or a 404 Not Found response if the employee does not exist.</returns>
        [HttpGet("/api/employees{id}")]
        public ActionResult<Employee> GetById(int id)
        {
            var employee = Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound(new { message = "Employee not found.", id });
            return employee;
        }

        /// <summary>
        /// Creates a new employee and adds it to the in-memory collection.
        /// </summary>
        /// <param name="employee">The employee object to create.</param>
        /// <returns>The created employee and a location header with the URI of the new resource.</returns>
        [HttpPost("/api/employees")]
        public ActionResult<Employee> Create(Employee employee)
        {
            employee.Id = Employees.Count > 0 ? Employees.Max(e => e.Id) + 1 : 1;
            Employees.Add(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        /// <summary>
        /// Updates the details of an existing employee.
        /// </summary>
        /// <param name="id">The ID of the employee to update.</param>
        /// <param name="updatedEmployee">The updated employee object.</param>
        /// <returns>The updated employee object,
        /// or a 404 Not Found response if the employee does not exist.</returns>
        [HttpPut("/api/employees/{id}")]
        public ActionResult<Employee> Update(int id, Employee updatedEmployee)
        {
            var employee = Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound();

            employee.Name = updatedEmployee.Name;
            employee.Position = updatedEmployee.Position;
            employee.Department = updatedEmployee.Department;
            employee.Salary = updatedEmployee.Salary;

            return Ok(employee);
        }

        /// <summary>
        /// Deletes an employee by their ID.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <returns>A 204 No Content response if the deletion is successful, 
        /// or a 404 Not Found response if the employee does not exist.</returns>
        [HttpDelete("/api/employees/{id}")]
        public ActionResult<Employee> Delete(int id)
        {
            var employee = Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound(new { message = "Employee not found", id });

            Employees.Remove(employee);
            return NoContent();
        }
    }
}
