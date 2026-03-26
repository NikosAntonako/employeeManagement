using System.ComponentModel.DataAnnotations;

namespace employeeManagement.Models
{
    /// <summary>
    /// Represents an Employee entity in the Employee Management application.
    /// This class is used to define the structure of the Employee data stored in memory during runtime.
    /// </summary>
    public class Employee
    {
        // ID is being set server side from Controller automatically
        // Cannot be set by POST, safe to not be required
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Position { get; set; }
        public required string Department { get; set; }
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal Salary { get; set; }
    }
}
