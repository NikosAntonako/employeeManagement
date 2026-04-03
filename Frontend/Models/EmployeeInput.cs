using System.ComponentModel.DataAnnotations;

namespace Frontend.Models;

public class EmployeeInput
{
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name cannot be empty.")]
    [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters and spaces.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Position is required.")]
    [MinLength(1, ErrorMessage = "Position cannot be empty.")]
    [StringLength(30, ErrorMessage = "Position cannot exceed 30 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Position must contain only letters and spaces.")]
    public string Position { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [MinLength(1, ErrorMessage = "Department  cannot be empty.")]
    [StringLength(30, ErrorMessage = "Department cannot exceed 30 characters.")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z\s]+$", ErrorMessage = "Department must contain only letters and spaces.")]
    public string Department { get; set; } = string.Empty;

    [Range(0.01, 1000000, ErrorMessage = "Salary must be between 0.01 and 1000000.")]
    public decimal Salary { get; set; }
}
