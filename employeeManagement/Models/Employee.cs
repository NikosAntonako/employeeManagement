using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace employeeManagement.Models
{
    /// <summary>
    /// Represents the database context for the Employee Management application.
    /// This class is used to configure and interact with the database using Entity Framework Core.
    /// </summary>
    public class EmployeeContext : DbContext
    {
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }
    }
    /// <summary>
    /// Represents an Employee entity in the Employee Management application.
    /// This class maps to the Employees table in the database.
    /// </summary>
    public class Employee
    {
        // Id is being set server side from Controller automatically
        // Cannot be set by POST, safe to not be required
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Position { get; set; }
        public required string Department { get; set; }
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }
    }
}
