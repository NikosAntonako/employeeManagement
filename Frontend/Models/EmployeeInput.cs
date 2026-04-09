using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

/// <summary>
/// Represents the input data required to create or update an employee record.
/// </summary>
public class EmployeeInput
{
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name cannot be empty.")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required.")]
    [MinLength(1, ErrorMessage = "Position cannot be empty.")]
    [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Position must contain only letters and spaces.")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [MinLength(1, ErrorMessage = "Department cannot be empty.")]
    [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Department must contain only letters and spaces.")]
    public string DepartmentName { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    [Range(0, 9999999.99, ErrorMessage = "Salary must be between 0 and 9,999,999.99.")]
    public decimal? Salary { get; set; }
}
