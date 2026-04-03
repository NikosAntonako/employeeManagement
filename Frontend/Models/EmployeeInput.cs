using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

/// <summary>
/// Represents the input data required to create or update an employee record.
/// </summary>
/// <remarks>This class is typically used for data transfer in scenarios such as API requests or form submissions.
/// All properties are required except for Salary, which must be a positive value within the specified range. Property
/// values are validated for length and character content to ensure data integrity.</remarks>
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
    [MinLength(1, ErrorMessage = "Department  cannot be empty.")]
    [StringLength(50, ErrorMessage = "Department cannot exceed 50 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Department must contain only letters and spaces.")]
    public string Department { get; set; } = string.Empty;

    [Range(0, 9999999.99, ErrorMessage = "Salary must be between 0 and 9999999,99.")]
    public decimal? Salary { get; set; }
}
