using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Represents the database context for the Employee Management application.
    /// This class is used to configure and interact with the database using Entity Framework Core.
    /// </summary>
    public class EmployeeContext(DbContextOptions<EmployeeContext> options) : DbContext(options)
    {
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

        [Required(ErrorMessage = "Name is required.")]
        [MinLength(1, ErrorMessage = "Name cannot be empty.")]
        [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        [MinLength(1, ErrorMessage = "Position cannot be empty.")]
        [StringLength(30, ErrorMessage = "Position cannot exceed 30 characters.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Position must contain only letters and spaces.")]
        public required string Position { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [MinLength(1, ErrorMessage = "Department cannot be empty.")]
        [StringLength(30, ErrorMessage = "Department cannot exceed 30 characters.")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Department must contain only letters and spaces.")]
        public required string Department { get; set; }

        [Range(0.01, 999999999999999999.99, ErrorMessage = "Salary must be between 0.01 and 999999999999999999.99.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }
    }
}
