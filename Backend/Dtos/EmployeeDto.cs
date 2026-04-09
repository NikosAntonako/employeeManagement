using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos;

/// <summary>
/// Represents a data transfer object containing basic information about an employee, including name, position,
/// department, and salary.
/// </summary>
public sealed record EmployeeDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name cannot be empty.")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Position is required.")]
    [MinLength(1, ErrorMessage = "Position cannot be empty.")]
    [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Position must contain only letters and spaces.")]
    public required string Position { get; set; }

    [Required(ErrorMessage = "Department is required.")]
    [MinLength(1, ErrorMessage = "Department cannot be empty.")]
    [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Department must contain only letters and spaces.")]
    public required string Department { get; set; }

    [Range(0, 9999999.99, ErrorMessage = "Salary must be between 0 and 9,999,999.99.")]
    public decimal? Salary { get; set; }
}
