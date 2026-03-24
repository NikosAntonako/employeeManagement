using employeeManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace employeeManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private static readonly List<Employee> Employees = [];

        /// <summary>
        /// GET /api/employees
        /// </summary>
        /// <returns>An enumerable collection of all employees. The collection will be empty if no employees are available.</returns>
        [HttpGet("/api/employees")]
        public IEnumerable<Employee> GetAll()
        {
            return Employees;
        }

        /// <summary>
        /// GET /api/employees/{id}
        /// </summary>
        /// <returns>The employee of the requested id.</returns>
        [HttpGet("/api/employees{id}")]
        public ActionResult<Employee> GetById(int id)
        {
            var employee = Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound(new { message = "Employee not found.", id });
            return employee;
        }

        /// <summary>
        /// Creates a new employee and adds it to the collection.
        /// </summary>
        /// <returns>An ActionResult containing the created employee and a location header with the URI of the new resource.</returns>
        [HttpPost("/api/employees")]
        public ActionResult<Employee> Create(Employee employee)
        {
            employee.Id = Employees.Count > 0 ? Employees.Max(e => e.Id) + 1 : 1;
            Employees.Add(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        /// <summary>
        /// Updates employee data of requested id
        /// </summary>
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
        /// Deletes employee of requested id
        /// </summary>
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
